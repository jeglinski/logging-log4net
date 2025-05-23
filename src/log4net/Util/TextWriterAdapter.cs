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
using System.Text;
using System.IO;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace log4net.Util;

/// <summary>
/// Adapter that extends <see cref="TextWriter"/> and forwards all
/// messages to an instance of <see cref="TextWriter"/>.
/// </summary>
/// <author>Nicko Cadell</author>
public abstract class TextWriterAdapter : TextWriter
{
  /// <summary>
  /// Creates an instance of <see cref="TextWriterAdapter"/> that forwards all
  /// messages to a <see cref="TextWriter"/>.
  /// </summary>
  /// <param name="writer">The <see cref="TextWriter"/> to forward to</param>
  protected TextWriterAdapter(TextWriter writer) : base(CultureInfo.InvariantCulture)
  {
    Writer = writer;
  }

  /// <summary>
  /// Gets or sets the underlying <see cref="TextWriter" />.
  /// </summary>
  protected TextWriter Writer { get; set; }

  /// <summary>
  /// The <see cref="Encoding"/> in which the output is written
  /// </summary>
  public override Encoding Encoding => Writer.Encoding;

  /// <summary>
  /// Gets an object that controls formatting
  /// </summary>
  public override IFormatProvider FormatProvider => Writer.FormatProvider;

  /// <summary>
  /// Gets or sets the line terminator string used by the TextWriter.
  /// </summary>
  [AllowNull]
  public override string NewLine
  {
    get => Writer.NewLine;
    set => Writer.NewLine = value;
  }

  /// <summary>
  /// Closes the writer and releases any system resources associated with the writer
  /// </summary>
  /// <remarks>
  /// <para>
  /// </para>
  /// </remarks>
  public override void Close() => Writer.Close();

  /// <summary>
  /// Dispose this writer
  /// </summary>
  /// <param name="disposing">flag indicating if we are being disposed</param>
  /// <remarks>
  /// <para>
  /// Dispose this writer
  /// </para>
  /// </remarks>
  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      (Writer as IDisposable)?.Dispose();
    }
  }

  /// <summary>
  /// Flushes any buffered output
  /// </summary>
  /// <remarks>
  /// <para>
  /// Clears all buffers for the writer and causes any buffered data to be written 
  /// to the underlying device
  /// </para>
  /// </remarks>
  public override void Flush() => Writer.Flush();

  /// <summary>
  /// Writes a character to the wrapped TextWriter
  /// </summary>
  /// <param name="value">the value to write to the TextWriter</param>
  /// <remarks>
  /// <para>
  /// Writes a character to the wrapped TextWriter
  /// </para>
  /// </remarks>
  public override void Write(char value) => Writer.Write(value);

  /// <summary>
  /// Writes a character buffer to the wrapped TextWriter
  /// </summary>
  /// <param name="buffer">the data buffer</param>
  /// <param name="index">the start index</param>
  /// <param name="count">the number of characters to write</param>
  /// <remarks>
  /// <para>
  /// Writes a character buffer to the wrapped TextWriter
  /// </para>
  /// </remarks>
  public override void Write(char[] buffer, int index, int count) => Writer.Write(buffer, index, count);

  /// <summary>
  /// Writes a string to the wrapped TextWriter
  /// </summary>
  /// <param name="value">the value to write to the TextWriter</param>
  /// <remarks>
  /// <para>
  /// Writes a string to the wrapped TextWriter
  /// </para>
  /// </remarks>
  public override void Write(string? value) => Writer.Write(value);
}
