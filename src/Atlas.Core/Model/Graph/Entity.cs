using System;

namespace Atlas.Core.Model.Graph.Entities;

public abstract record Entity(string Name, IEnumerable<Relationship> Link);
