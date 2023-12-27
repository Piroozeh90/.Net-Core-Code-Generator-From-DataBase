﻿using System;
using System.Collections.Generic;
using System.Linq;
using BPC.CodeGenerator.Metadata.Parsing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BPC.CodeGenerator.Parsing
{
    public class RegionVisitor : CSharpSyntaxWalker
    {
        private readonly Stack<CodeRegion> _stack = new Stack<CodeRegion>();

        public RegionVisitor() : base(SyntaxWalkerDepth.StructuredTrivia)
        {
            Regions = new Dictionary<string, CodeRegion>(StringComparer.OrdinalIgnoreCase);
        }

        public Dictionary<string, CodeRegion> Regions { get; }

        public override void VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
        {
            var region = new CodeRegion
            {
                StartIndex = node.FullSpan.Start,
                Name = ParseRegionName(node)
            };
            _stack.Push(region);

            base.VisitRegionDirectiveTrivia(node);
        }

        public override void VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
        {
            if (_stack.Count == 0)
                return;

            var region = _stack.Pop();
            region.EndIndex = node.FullSpan.End;

            Regions[region.Name] = region;

            base.VisitEndRegionDirectiveTrivia(node);
        }

        private string ParseRegionName(RegionDirectiveTriviaSyntax node)
        {
            var preprocessingMessage = node
                .DescendantTrivia()
                .FirstOrDefault(t => t.Kind() == SyntaxKind.PreprocessingMessageTrivia);


            return preprocessingMessage.ToString();
        }
    }
}