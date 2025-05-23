#region Apache License
//
// Licensed to the Apache Software Foundation (ASF) under one or more 
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership. 
// The ASF licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with 
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System.Diagnostics.CodeAnalysis;
using System.IO;

using log4net.Core;

namespace log4net.Layout.Pattern;

/// <summary>
/// Writes the value of the <see cref="LocationInfo.FileName"/> for
/// the event to the output writer.
/// </summary>
/// <author>Nicko Cadell</author>
[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Reflection")]
internal sealed class FileLocationPatternConverter : PatternLayoutConverter
{
  /// <summary>
  /// Writes the value of the <see cref="LocationInfo.FileName"/> for
  /// the <paramref name="loggingEvent"/> to the output <paramref name="writer"/>.
  /// </summary>
  /// <param name="writer"><see cref="TextWriter" /> that will receive the formatted result.</param>
  /// <param name="loggingEvent">the event being logged</param>
  protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
  {
    if (loggingEvent.LocationInformation is not null)
    {
      writer.Write(loggingEvent.LocationInformation.FileName);
    }
  }
}
