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

using System.IO;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace log4net.Util.PatternStringConverters;

/// <summary>
/// AppSetting pattern converter
/// </summary>
/// <remarks>
/// <para>
/// This pattern converter reads appSettings from the application configuration file.
/// </para>
/// <para>
/// If the <see cref="PatternConverter.Option"/> is specified then that will be used to
/// lookup a single appSettings value. If no <see cref="PatternConverter.Option"/> is specified
/// then all appSettings will be dumped as a list of key value pairs.
/// </para>
/// <para>
/// A typical use is to specify a base directory for log files, e.g.
/// <example>
/// <![CDATA[
/// <log4net>
///    <appender name="MyAppender" type="log4net.Appender.RollingFileAppender">
///      <file type="log4net.Util.PatternString" value="appsetting{LogDirectory}MyApp.log"/>
///       ...
///   </appender>
/// </log4net>
/// ]]>
/// </example>
/// </para>
/// </remarks>
[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Reflection")]
internal sealed class AppSettingPatternConverter : PatternConverter
{
  private static IDictionary AppSettingsDictionary
  {
    get
    {
      if (_appSettings is null)
      {
        Dictionary<string, string?> result = [];
        foreach (string key in ConfigurationManager.AppSettings)
        {
          result.Add(key, ConfigurationManager.AppSettings[key]);
        }
        _appSettings = result;
      }
      return _appSettings;
    }

  }
  private static Dictionary<string, string?>? _appSettings;

  /// <summary>
  /// Write the property value to the output
  /// </summary>
  /// <param name="writer"><see cref="TextWriter" /> that will receive the formatted result.</param>
  /// <param name="state">null, state is not set</param>
  /// <remarks>
  /// <para>
  /// Writes out the value of a named property. The property name
  /// should be set in the <see cref="log4net.Util.PatternConverter.Option"/>
  /// property.
  /// </para>
  /// <para>
  /// If the <see cref="log4net.Util.PatternConverter.Option"/> is set to <c>null</c>
  /// then all the properties are written as key value pairs.
  /// </para>
  /// </remarks>
  public override void Convert(TextWriter writer, object? state)
  {
    if (Option is not null)
    {
      // Write the value for the specified key
      WriteObject(writer, null, ConfigurationManager.AppSettings[Option]);
    }
    else
    {
      // Write all the key value pairs
      WriteDictionary(writer, null, AppSettingsDictionary);
    }
  }
}