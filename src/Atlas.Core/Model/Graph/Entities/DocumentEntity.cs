using System;

namespace Atlas.Core.Model.Graph.Entities;

public record DocumentEntity(string Name, string Link) : Entity(Name, Link);
