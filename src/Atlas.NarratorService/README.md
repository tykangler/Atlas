# Atlas.NarratorService

A gRPC service that extracts semantic relationships from text using spaCy's NLP engine and dependency parsing.

## Overview

The NarratorService identifies how entities (target phrases) relate to actions (verbs) in sentences by analyzing the dependency parse tree. It answers the question: **"What verb governs this phrase, and how?"**

For example, if analyzing "John is the manager of engineering", with target phrase "manager of engineering":
- The relationship returned will be `is` (action)
- The qualifier will be `of` (prep)
- The detailed action will be `is manager of engineering`

Refer to the [jupyter notebook](../../lab/relationships.ipynb) for details on implementation, rules, and experiments.

## Architecture

### Components

**server.py** - gRPC server setup:
- Creates a gRPC server with 10 worker threads
- Registers `NarratorService` as the servicer
- Listens on port 50051
- Currently runs without SSL (insecure)

**narrator_service.py** - Core NLP logic:
- Loads spaCy's `en_core_web_lg` model for English language processing
- Implements the `GetRelationships` gRPC method
- Uses dependency parsing to extract semantic relationships

## How It Works

### The Processing Flow

#### 1. **GetRelationships(request, context)** - Entry Point

Receives a `DocumentRequest` and streams back `RelationshipResponse` objects.

**Input:** `DocumentRequest` containing:
- `text`: The document content to analyze
- `targetPhrases`: List of phrases (entities) to analyze, each with:
  - `text`: The phrase text
  - `startIndex`: Character position where phrase starts
  - `endIndex`: Character position where phrase ends

**Output:** Streams `RelationshipResponse` objects (yields results one at a time)

#### 2. **process_extracts(requests)** - Batch NLP Processing

**Process:**
1. Uses `nlp.pipe()` to efficiently batch-process documents through spaCy
2. For each target phrase in the document:
   - Locates the phrase in the spaCy `Doc` as a `Span` using character positions
   - Analyzes that span to find its relationship to verbs

#### 3. **get_phrase_span(doc, phrase, start_index, end_index)** - Locate the Phrase

Converts character positions from the original text into a **spaCy Span** object.

The Span provides access to:
- `span.root`: The syntactic head of the phrase (the main token)
- `span.subtree`: All tokens that depend on the root
- Linguistic properties (POS tags, dependency labels, etc.)

#### 4. **evaluate_relationships(span)** - Core Relationship Extraction

This is where the relationship extraction happens using **dependency parsing**.

##### Case 1: The phrase root IS a verb

If the target phrase's root token is a verb or auxiliary verb, return it directly:
- `action` = the verb text
- `prep` = None
- `detailed_action` = all tokens in the phrase's subtree
- `target` = the original phrase text

**Example:** "created the database"
- Result: action="created", prep=None, detailed_action="created the database"

##### Case 2: The phrase root is NOT a verb - walk up the dependency tree

If the root is not a verb, walk up the dependency tree (ancestors) to find the governing verb:

1. Track conjunctions to avoid including unrelated parts of the sentence
2. Capture prepositions (ADP) along the way
3. Add intermediate tokens to build the detailed action
4. Stop when a verb is found

**Example:** "manager of engineering"
- Walk up: "manager" → "of" (capture prep) → "is" (found verb!)
- Result: action="is", prep="of", detailed_action="is manager of engineering"

**Conjunction Handling:** The algorithm skips tokens that are part of a different conjunct to avoid including unrelated parts of the sentence.

### Output: RelationshipResponse

Each response contains:
- **action**: The main verb (e.g., "is", "created", "manages")
- **prep**: Preposition qualifier (e.g., "of", "in", "with") - can be `None`
- **detailed_action**: Full verb phrase including modifiers (e.g., "is manager of")
- **target**: The original target phrase text

## NLP Concepts Used

### 1. Dependency Parsing

spaCy builds a tree showing how words depend on each other grammatically:
- `span.root`: The syntactic head of a phrase
- `span.root.ancestors`: Parents in the dependency tree (tokens this phrase depends on)
- `span.subtree`: Children in the dependency tree (tokens that depend on this phrase)

### 2. POS Tags (Part of Speech)

- `VERB`, `AUX`: Verbs and auxiliary verbs (is, was, created, etc.)
- `ADP`: Adpositions (prepositions like "of", "in", "with")
- `NOUN`, `PROPN`: Nouns

### 3. Dependency Labels

- `conj`: Conjunction relationships (and, or)
- Used to filter out parallel structures and avoid noise

## Example Walkthrough

**Input:**
```json
{
  "text": "John is the manager of engineering at Google",
  "targetPhrases": [
    {
      "text": "manager of engineering",
      "startIndex": 12,
      "endIndex": 34
    }
  ]
}
```

**Processing:**
1. spaCy parses the sentence and builds dependency tree:
   ```
   [John] ←(nsubj)← [is] →(attr)→ [manager] →(prep)→ [of] →(pobj)→ [engineering]
   ```

2. Locate the span for "manager of engineering"
   - `span.root` = "manager" (NOUN)

3. Walk up ancestors to find governing verb:
   - "of" (ADP) → set `prep = "of"`
   - "is" (VERB) → **Found verb!**

4. Build token list: ["is", "manager", "of", "engineering"]

**Output:**
```protobuf
RelationshipResponse {
  action: "is"
  prep: "of"
  detailed_action: "is manager of engineering"
  target: "manager of engineering"
}
```

## Setup and Usage

### Prerequisites
- Python 3.11.11
- Make (for build automation)

### Installation

```bash
# Navigate to the service directory
cd src/Atlas.NarratorService

# Install dependencies including spaCy and en_core_web_lg model
make setup

# Generate gRPC stub files from proto definitions
make generate

# Or run both setup and generate
make build
```

### Running the Service

```bash
# Start the gRPC server on port 50051
python server.py

# Or use make
make run
```

The service will listen on `[::]:50051` (all interfaces, port 50051).

## Design Trade-offs

### Strengths ✅
- Leverages spaCy's powerful dependency parser
- Handles complex sentence structures
- Streams results efficiently via gRPC
- Batch processing capability with `nlp.pipe()`

### Limitations ⚠️
- Only finds relationships for **pre-identified target phrases** (entities must be provided)
- Relies on spaCy's parsing accuracy (can misparse complex or ambiguous sentences)
- Conjunction handling is basic (may miss some edge cases)
- Currently processes one request at a time despite batch processing capability
- No SSL/TLS encryption (insecure for production)

## Implementation Details

### State Machine

| state (including root) | is_conj | add_token? | next_state   | set prep token? |
|------------------------|---------|------------|--------------|-----------------|
| pos=verb               | false   | yes        | break        | no              |
| pos=verb               | true    | yes        | break        | no              |
| pos=aux                | false   | yes        | break        | no              |
| pos=aux                | true    | yes        | break        | no              |
| dep=conj               | false   | yes        | is_conj=true | no              |
| dep=conj               | true    | no         | is_conj=true | no              |
| pos=adp                | false   | yes        | next         | yes             |
| pos=adp                | true    | no         | is_conj=false| yes             |
| _                      | false   | yes        | next         | no              |
| _                      | true    | no         | is_conj=false| no              |

## Messaging

This project will use GRPC for communication and messaging. (just to try something new). The gRPC service will take in a single `DocumentRequest` and output a stream of `Relationship`. 
`DocumentRequest` can be either a long document or a document extract. 

### Chunking

We will be sending either:
1. the entire document (parsed text + phrases)
2. document chunks (parsed text + phrases)

Let's go with option 2. There's no need to stream the document chunks.

### Formats

#### Input

```protobuf
message DocumentRequest {
    string text = 1;
    repeated Phrase targetPhrases = 2;
}

message Phrase {
    int32 startIndex = 1;
    int32 endIndex = 2;
    string text = 3;
}
```

#### Output

```protobuf
message RelationshipResponse {
    string action = 1;
    string prep = 2;
    string detailed_action = 3;
    string target = 4;
}
```

#### Service Definition

```protobuf
service Narrator {
    rpc GetRelationships(DocumentRequest) returns (stream RelationshipResponse);
}
```

