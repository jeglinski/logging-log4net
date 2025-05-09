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

using System;

using log4net.Core;
using log4net.Util;

namespace log4net.Layout;

/// <summary>
/// Extracts the date from the <see cref="LoggingEvent"/>.
/// </summary>
/// <author>Nicko Cadell</author>
/// <author>Gert Driesen</author>
public class RawUtcTimeStampLayout : IRawLayout
{
  /// <summary>
  /// Gets the <see cref="LoggingEvent.TimeStamp"/> as a <see cref="DateTime"/>.
  /// </summary>
  /// <param name="loggingEvent">The event to format</param>
  /// <returns>returns the time stamp</returns>
  /// <remarks>
  /// <para>
  /// The time stamp is in universal time. To format the time stamp
  /// in local time use <see cref="RawTimeStampLayout"/>.
  /// </para>
  /// </remarks>
  public virtual object Format(LoggingEvent loggingEvent) 
    => loggingEvent.EnsureNotNull().TimeStampUtc;
}
