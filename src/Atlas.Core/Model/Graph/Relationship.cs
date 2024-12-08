using System;
using AngleSharp.Dom;

namespace Atlas.Core.Model.Graph;

public record Relationship(Entity From, Entity To);
