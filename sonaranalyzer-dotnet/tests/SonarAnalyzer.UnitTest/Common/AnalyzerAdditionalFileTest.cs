/*
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SonarAnalyzer.Common;
using FluentAssertions;

using Google.Protobuf;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SonarAnalyzer.Helpers;
using SonarAnalyzer.Protobuf;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SonarAnalyzer.UnitTest.Common
{
    [TestClass]
    public class AnalyzerAdditionalFileTest
    {
        [TestMethod]
        public void AnalyzerAdditionalFile_GetText()
        {
            var additionalFile = new AnalyzerAdditionalFile(@"Common\Resources\input.txt");
            var content = additionalFile.GetText();
            content.ToString().Should().Be("some sample content");
        }

        [TestMethod]
        public void FIXME_REMVOE_DEBUG() //FIXME: REMOVE DEBUG
        {
            var parser = CopyPasteTokenInfo.Parser;
            var lst = new List<CopyPasteTokenInfo>();
            CopyPasteTokenInfo item;
            var fn = @"c:\_temp\IndexOfOutRangeProtobuf\output-cs0\token-cpd.pb";
            using (var fs = new FileStream(fn, FileMode.Open))
            {
                while (true)
                {
                    try
                    {
                        item = parser.ParseDelimitedFrom(fs);
                        if(item.FilePath== @"D:\bamboo-home\xml-data\build-dir\SMR-EITSAPMASTERDATA-JOB1\Source\SettingsManager\StringCipher.cs")
                        {
                            var sb = new StringBuilder();
                            var sbLine = new StringBuilder();
                            var range = new StringBuilder();
                            int lineCnt = 1;
                            foreach(var token in item.TokenInfo)
                            {
                                var r = token.TextRange;
                                while (lineCnt < r.StartLine) //StartLine is LineNumber, not LineIndex
                                {
                                    sbLine.AppendLine();
                                    sb.Append(sbLine);
                                    sbLine.Clear();
                                    lineCnt++;
                                }
                                while (sbLine.Length < r.StartOffset)
                                {
                                    sbLine.Append(' ');
                                }
                                sbLine.Append(token.TokenValue);
                                if (r.StartLine == r.EndLine)
                                {
                                    range.AppendLine($"{r.StartLine}: {r.StartOffset}-{r.EndOffset}: {token.TextRange}");
                                }
                                else
                                {
                                    range.AppendLine($"{r.StartLine}: {r.StartOffset} to {r.EndLine}: {r.EndOffset}: {token.TextRange}");
                                }
                            }
                            sb.Append(sbLine).AppendLine();
                            File.WriteAllText(@"c:\_temp\IndexOfOutRangeProtobuf\tokens.cs", sb.ToString());
                            File.WriteAllText(@"c:\_temp\IndexOfOutRangeProtobuf\ranges.txt", range.ToString());
                            break;
                        }
                    } catch(InvalidProtocolBufferException ex)
                    {
                        break;
                    }
                }
            }
            System.Diagnostics.Debugger.Break();
        }
    }
}
