# Atlas.RelationshipMapper

This project will take in a sequence of text, an article title, and a target phrase. It will find the relationship between
the article title and the target phrase with the information provided in the text. The relationship is expressed as a single verb,
and will include any qualifiers for the verb. For example, if the article title was a member of a group. The relationship returned will be
`is`, and the qualifier may be `of`, if the target phrase is the `group name`.

Refer to the [jupyter notebook](../../lab/relationships.ipynb) for details on implementation, rules, and experiments.

## Rules

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

This project will use GRPC for communication and messaging. (just to try something new). The gRPC service will take in a stream of `DocumentRequest` and output a stream of `Relationship`. Note 
that ordering is preserved.

### Formats

#### Input

```protobuf
message DocumentRequest {
    string text = 1;
    string subject = 2;
    repeated Phrase targetPhrases = 3;
}
```

#### Output

```protobuf
message Relationship {
    string subject = 1;
    string action = 2;
    string prep = 3;
    string detailed_action = 4;
    string target = 5;
}
```

