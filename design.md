# Design of Atlas

## Overview

Atlas is a project that, in a way similar to Google's Knowledge Graph, will map knowledge "entities" and their
relationships to each other. Assuming we have entities "Zeus", "Poseidon", and "Greek Gods", a query on Zeus
will reveal that he is part of the category "Greek Gods", along with "Poseidon." It will also reveal that 
"Zeus" and "Poseidon" are brothers, and that he gave birth to Heracles, who is a demigod. If we wanted to reveal the relationship betwen Theseus and Heracles, we can see that they're both demigods. And if we wanted to see the relationship between Zeus and Theseus, we can see that Theseus is Zeus' brother's (Poseidon) son. In other words, he is his uncle. \
We should be able to print this information in both paragraph form, and graph form. For example, "Zeus is a greek god, and is Theseus' uncle."

```mermaid
flowchart LR
   P((Poseidon))---|Is a member of|G((Greek Gods)) ---|Is a member of|Z((Zeus));
   H((Hades))---|Is a member of|G
   P---|Brothers|Z---|Brothers|H
   DG((Demigods))---|Is a member of|He((Heracles))---|Son of|Z
   DG---|Is a member of|Th((Theseus))---|Son of|P
```

## Design Considerations
* **Many Relationship Types:** \
   We will need to model many different kinds of relationships, 
   and these relationship types may be **mutually exclusive**, depending on the entity type. 
   For example, Poseidon can be classified as a **Person** entity. 
   He may have family relationships, 'has killed' relationships, 'has invented' relationships. 
   But he won't have a 'was written by' relationship, something that might be 
   exclusive to books/music/scores. Relationship types may also be **shared** between entity 
   types. The "is a member of" relationship will most likely be a part of all entity types, 
   though the exact label may change.

   These relationships can be concrete/hard-coded, but they could also consist of only 
   labels/strings to be more flexible.

* **Many Entity Types**: \
   We should be able to recognize different types of entities (Books, People, Music, 
   Movies, Characters, Places). These entities will have different characteristics, and 
   should be treated as separate classifications.

* **Multiple relationships**: \
   We can recognize more than one type of relationship between entity A and entity B. 
   i.e. The relationship between Luke Skywalker and Darth Vader is complex. Darth Vader is 
   not just Luke's father, but also his main antagonist. Luke also killed Darth Vader.

   Relationships may also be directed, or undirected. The 'member' relationship is useful
   as an undirected relationship in case  we'd want to list all members of a category,
   or see whether an entity is part of a category. The 'father-son' relationship should
   be a directed relationship to see who is the father and who is the son. However, if we want
   to find the father from the son, this will be difficult. We could store parent along with
   children edges.

* **Language Model**: \
   We will need to parse through text to identify the relationship between two entities. 
   Given a fragment like "Originally a farmer on Tatooine living with his uncle and aunt", in an
   article about "Luke Skywalker", We should recognize that Luke Skywalker lived on Tatooine.
   Looking at info boxes could make this simpler. 

   Should also recognize when a sentence is not relevant to the article. The sentence 
   "He places him in his X-wing starfighter, which is then flown to Tatooine by R2-D2" is
   irrelevant.

* **Data Processing**: \
   Main data source will be wikipedia's data dumps. These files are very large, so we should
   start with a partition to build a small representation. After testing, expand to 
   other partitions. Could also download, and unzip online, building one sub-component
   at a time, serializing the data we need, and throwing away the rest.

* **Output**: \
   Should be able to output in both paragraph form, and relationship form. Maybe later, 
   I'll expand to include a graph visualized form. 

* **Relationship Querying:** \
   When querying two entities for relationship data, should first find the shortest connection
   possible. If the user wants, find the next shortest connection. So and so forth. When querying
   using an entity and a relationship (who is Luke's mother?), the relationship may have different
   names in the system. Should find either the closest matching one, or say no relationship. 

## Implementation

Atlas will use a multigraph model, with nodes set to knowledge entities, and edges set 
to relationships.

### Entities

Entites are nodes. They should convey name, and type, and definition. Each entity should be able 
to store incoming and outgoing edges.

```csharp
public class Entity {
   EntityType type;
   string name;
   string definition;
   ICollection<Relationship> relations; // storing both incoming and outgoing
}

// or

public abstract class Entity {
   // ...
}

public class BookEntity : Entity {
   string name;
   string definition;
   ICollection<Relationship> relations;
}
```

### Relationships

Relationships are edges. They should allow traversal from v1 to v2 and the reverse, v2 to v1. 
Because we need to maintain multiple edges between two nodes, we can have either the entity 
carry the multiple edges, or have a list representing relationships in a single edge class.

```csharp
public class Relationship {
   ICollection<RelationshipType> relations;
   Entity parent;
   Entity child;
}

// or 

public class Entity {
   ICollection<Relationship> relations; // this will be here anyways
   // but in addition to storing unique edges, will also store 
   // same edges.
   // ...
}

public class Relationship {
   Entity parent;
   Entity child;
}
```

The first approach is cleaner in that all relations between v1 and v2 are stored in one place.
It however incurs overhead in that it has to store another collection. The second
approach avoids this overhead, but must implement more complex logic for queries.

### Language Model

Sentences are complex. This may be most complex part. Atlas must parse sentences to retrieve
a "from" entity, a "to" entity, and their relationship. It must also be able to disregard 
sentences that contain both the "from" and "to" entities, but has an irrelevant relationship.
In the wiki article, "MultiGraph", the sentence "A multigraph is different from a hypergraph."
The two entities are "multigraph" and "hypergraph", but the relationship is "different." 
We might decide not to include this edge at all.  

#### Characteristics of Sentences

* Each sentence specifies the subject at least once. Sentences may have more than one subject
connected by clauses, or the subject may encompass multiple entities. "John and Ann went to
the store." "John went to the store, and Ann went to the park".

* Object may not be specified. "She sang beautifully." "He worked hard."

* Subject can be replaced by a pronoun, "he", "she". In this case, the subject is implicitly
defined depending on the context. If the prior sentences talked about "Jane Austen", 
the "she" in the following sentence is referencing Jane Austen. 

* Independent Clauses and dependent clauses

#### First steps/Basic parsing

We will need to implement a grammar model, and parse through sentences using this model to 
identify at least a subject, a verb, and a possible object. If there is no object, we can
throw the sentence, and assume there is no relationship. The verb will represent the
relationship. We'll have to identify whether the sentence is passive or active, as that can
affect the id of V<sub>from</sub> and V<sub>to</sub>. 

Initial idea was to create a context free grammar and a language parser to accompany,
but maybe that's too complex. Knowing the subject and object makes things
simpler. A context free grammar would require having a large list of terminals.
We can identify stop words, prepositional words, and identify relationships
with the words around subject and object using our pre-defined relationships. Terminal words
would essentially be relationships, objects/targets, and subjects.

By starting with pre-defined relationship types, we can also just look for those words 
instead of id'ing every part of the sentence.  

First implement this in Jupyter Lab.

#### Capabilities

**Keywords**: We can define **keywords** for each relationship type. For example, the "member" relationship
can have keywords: "is an/a", "is a type of", "was an". We can take what's on the left, define 
as "from", and what's on the right, define as "to". This will work for **simple sentences**
and will require finding the relevant keywords manually.

**Boolean Clauses**: Should be able to recognize "and"s, "or"s, and "not"s. The exact word used
may differ. "Northanger Abbey is a coming-of-age novel **and** a satire of Gothic novels".

**Info boxes**: Wiki's info boxes are an easy/quick way to parse information. In the entry for 
"Northanger Abbey", we can see it's followed by "Persuasion." Similar relationships can be consistently
and easily parsed. 

**Implicitly matching entities**: In many cases, V<sub>from</sub>, or V<sub>to</sub> may
not be explicitly mentioned in the same sentence. Instead a pronoun is used (he/she/it). 
We need to **analyze context** to find the meaning of the pronoun. If it is a relevant entity, 
further analyze the sentence.

**Dependent Clauses**: the information may be present in a dependent clause beginning the sentence.
"Born in Tatooine, Luke Skywalker ...". This sentence establishes a relationship between Luke
Skywalker and Tatooine. Context should be determined to find subject. For example,
"Born in Tatooine, He ..." should also match Luke Skywalker and Tatooine. 

#### Context Matching

If pronoun, or indirect subject/object, found, then check preceding sentences for subject match.
Indirect objects can be adding all nodes first with associated URLs, and checking whether
the object has a link to an existing node. 

For indirect subjects, if 'he/she/they/him/her/them' check for person(s). If 'it/they/them' check 
for things(s). Should be simple to check if object or person by looking at wiki categories.

If referencing a subject that isn't the article's subject, ignore.

#### Simplifying expressions

Brothers -- Son -> Uncle
