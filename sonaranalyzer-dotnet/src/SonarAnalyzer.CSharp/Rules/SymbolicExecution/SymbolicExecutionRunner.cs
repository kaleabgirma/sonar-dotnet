﻿/*
 * SonarAnalyzer for .NET
 * Copyright (C) 2015-2020 SonarSource SA
 * mailto: contact AT sonarsource DOT com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SonarAnalyzer.Common;
using SonarAnalyzer.Helpers;
using SonarAnalyzer.Rules.CSharp;
using SonarAnalyzer.SymbolicExecution;

namespace SonarAnalyzer.Rules.SymbolicExecution
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [Rule(EmptyNullableValueAccess.DiagnosticId)]
    [Rule(ObjectsShouldNotBeDisposedMoreThanOnce.DiagnosticId)]
    [Rule(PublicMethodArgumentsShouldBeCheckedForNull.DiagnosticId)]
    public sealed class SymbolicExecutionRunner : SonarDiagnosticAnalyzer
    {
        private readonly SymbolicExecutionAnalyzerFactory symbolicExecutionAnalyzerFactory = new SymbolicExecutionAnalyzerFactory();

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public SymbolicExecutionRunner()
        {
            SupportedDiagnostics = this.symbolicExecutionAnalyzerFactory.SupportedDiagnostics;
        }

        protected override void Initialize(SonarAnalysisContext context)
        {
            context.RegisterExplodedGraphBasedAnalysis(Analyze);
        }

        private void Analyze(CSharpExplodedGraph explodedGraph, SyntaxNodeAnalysisContext context)
        {
            var analyzerContexts = InitializeAnalyzers(explodedGraph, context).ToList();

            try
            {
                explodedGraph.Walk();
            }
            finally
            {
                foreach (var diagnostic in analyzerContexts.SelectMany(analyzerContext => analyzerContext.GetDiagnostics()))
                {
                    context.ReportDiagnosticWhenActive(diagnostic);
                }

                analyzerContexts.ForEach(analyzerContext => analyzerContext.Dispose());
            }
        }

        private IEnumerable<ISymbolicExecutionAnalysisContext> InitializeAnalyzers(CSharpExplodedGraph explodedGraph, SyntaxNodeAnalysisContext context) =>
            this.symbolicExecutionAnalyzerFactory
                .GetEnabledAnalyzers(context)
                .Select(analyzer => analyzer.AddChecks(explodedGraph, context));
    }
}
