using System;

namespace Atlas.Core.Model.Graph.Entities;

public record CategoryEntity(string Name, string Link) : Entity(Name, Link);
