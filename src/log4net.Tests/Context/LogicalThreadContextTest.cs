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
using System.Threading.Tasks;
using System.Linq;

using log4net.Config;
using log4net.Layout;
using log4net.Repository;
using log4net.Tests.Appender;
using log4net.Util;

using NUnit.Framework;

namespace log4net.Tests.Context;

/// <summary>
/// Used for internal unit testing the <see cref="LogicalThreadContext"/> class.
/// </summary>
/// <remarks>
/// Used for internal unit testing the <see cref="LogicalThreadContext"/> class.
/// </remarks>
[TestFixture]
public class LogicalThreadContextTest
{
  [TearDown]
  public void TearDown() => Utils.RemovePropertyFromAllContexts();

  [Test]
  public void TestLogicalThreadPropertiesPatternBasicGetSet()
  {
    StringAppender stringAppender = new()
    {
      Layout = new PatternLayout("%property{" + Utils.PropertyKey + "}")
    };

    ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
    BasicConfigurator.Configure(rep, stringAppender);

    ILog log1 = LogManager.GetLogger(rep.Name, "TestLogicalThreadPropertiesPattern");

    log1.Info("TestMessage");
    Assert.That(stringAppender.GetString(), Is.EqualTo(SystemInfo.NullText), "Test no logical thread properties value set");
    stringAppender.Reset();

    LogicalThreadContext.Properties[Utils.PropertyKey] = "val1";

    log1.Info("TestMessage");
    Assert.That(stringAppender.GetString(), Is.EqualTo("val1"), "Test logical thread properties value set");
    stringAppender.Reset();

    LogicalThreadContext.Properties.Remove(Utils.PropertyKey);

    log1.Info("TestMessage");
    Assert.That(stringAppender.GetString(), Is.EqualTo(SystemInfo.NullText), "Test logical thread properties value removed");
    stringAppender.Reset();
  }

  [Test]
  public async Task TestLogicalThreadPropertiesPatternAsyncAwait()
  {
    StringAppender stringAppender = new()
    {
      Layout = new PatternLayout("%property{" + Utils.PropertyKey + "}")
    };

    ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
    BasicConfigurator.Configure(rep, stringAppender);

    ILog log1 = LogManager.GetLogger(rep.Name, "TestLogicalThreadPropertiesPattern");

    log1.Info("TestMessage");
    Assert.That(stringAppender.GetString(), Is.EqualTo(SystemInfo.NullText), "Test no logical thread stack value set");
    stringAppender.Reset();

    string testValueForCurrentContext = "Outer";
    LogicalThreadContext.Properties[Utils.PropertyKey] = testValueForCurrentContext;

    log1.Info("TestMessage");
    Assert.That(stringAppender.GetString(), Is.EqualTo(testValueForCurrentContext), "Test logical thread properties value set");
    stringAppender.Reset();

    string[] strings = await Task.WhenAll(Enumerable.Range(0, 10).Select(x => SomeWorkProperties(x.ToString()))).ConfigureAwait(false);

    // strings should be ["00AA0BB0", "01AA1BB1", "02AA2BB2", ...]
    for (int i = 0; i < strings.Length; i++)
    {
      Assert.That(strings[i], Is.EqualTo(string.Format("{0}{1}AA{1}BB{1}", testValueForCurrentContext, i)), "Test logical thread properties expected sequence");
    }

    log1.Info("TestMessage");
    Assert.That(stringAppender.GetString(), Is.EqualTo(testValueForCurrentContext), "Test logical thread properties value set");
    stringAppender.Reset();

    LogicalThreadContext.Properties.Remove(Utils.PropertyKey);

    log1.Info("TestMessage");
    Assert.That(stringAppender.GetString(), Is.EqualTo(SystemInfo.NullText), "Test logical thread properties value removed");
    stringAppender.Reset();
  }

  [Test]
  public void TestLogicalThreadStackPattern()
  {
    StringAppender stringAppender = new()
    {
      Layout = new PatternLayout("%property{" + Utils.PropertyKey + "}")
    };

    ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
    BasicConfigurator.Configure(rep, stringAppender);

    ILog log1 = LogManager.GetLogger(rep.Name, "TestThreadStackPattern");

    log1.Info("TestMessage");
    Assert.That(stringAppender.GetString(), Is.EqualTo(SystemInfo.NullText), "Test no logical thread stack value set");
    stringAppender.Reset();

    using (LogicalThreadContext.Stacks[Utils.PropertyKey].Push("val1"))
    {
      log1.Info("TestMessage");
      Assert.That(stringAppender.GetString(), Is.EqualTo("val1"), "Test logical thread stack value set");
      stringAppender.Reset();
    }

    log1.Info("TestMessage");
    Assert.That(stringAppender.GetString(), Is.EqualTo(SystemInfo.NullText), "Test logical thread stack value removed");
    stringAppender.Reset();
  }

  [Test]
  public void TestLogicalThreadStackPattern2()
  {
    StringAppender stringAppender = new()
    {
      Layout = new PatternLayout("%property{" + Utils.PropertyKey + "}")
    };

    ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
    BasicConfigurator.Configure(rep, stringAppender);

    ILog log1 = LogManager.GetLogger(rep.Name, "TestThreadStackPattern");

    log1.Info("TestMessage");
    Assert.That(stringAppender.GetString(), Is.EqualTo(SystemInfo.NullText), "Test no logical thread stack value set");
    stringAppender.Reset();

    using (LogicalThreadContext.Stacks[Utils.PropertyKey].Push("val1"))
    {
      log1.Info("TestMessage");
      Assert.That(stringAppender.GetString(), Is.EqualTo("val1"), "Test logical thread stack value set");
      stringAppender.Reset();

      using (LogicalThreadContext.Stacks[Utils.PropertyKey].Push("val2"))
      {
        log1.Info("TestMessage");
        Assert.That(stringAppender.GetString(), Is.EqualTo("val1 val2"), "Test logical thread stack value pushed 2nd val");
        stringAppender.Reset();
      }
    }

    log1.Info("TestMessage");
    Assert.That(stringAppender.GetString(), Is.EqualTo(SystemInfo.NullText), "Test logical thread stack value removed");
    stringAppender.Reset();
  }

  [Test]
  public void TestLogicalThreadStackPatternNullVal()
  {
    StringAppender stringAppender = new()
    {
      Layout = new PatternLayout("%property{" + Utils.PropertyKey + "}")
    };

    ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
    BasicConfigurator.Configure(rep, stringAppender);

    ILog log1 = LogManager.GetLogger(rep.Name, "TestThreadStackPattern");

    log1.Info("TestMessage");
    Assert.That(stringAppender.GetString(), Is.EqualTo(SystemInfo.NullText), "Test no logical thread stack value set");
    stringAppender.Reset();

    using (LogicalThreadContext.Stacks[Utils.PropertyKey].Push(null))
    {
      log1.Info("TestMessage");
      Assert.That(stringAppender.GetString(), Is.EqualTo(SystemInfo.NullText), "Test logical thread stack value set");
      stringAppender.Reset();
    }

    log1.Info("TestMessage");
    Assert.That(stringAppender.GetString(), Is.EqualTo(SystemInfo.NullText), "Test logical thread stack value removed");
    stringAppender.Reset();
  }

  [Test]
  public void TestLogicalThreadStackPatternNullVal2()
  {
    StringAppender stringAppender = new()
    {
      Layout = new PatternLayout("%property{" + Utils.PropertyKey + "}")
    };

    ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
    BasicConfigurator.Configure(rep, stringAppender);

    ILog log1 = LogManager.GetLogger(rep.Name, "TestThreadStackPattern");

    log1.Info("TestMessage");
    Assert.That(stringAppender.GetString(), Is.EqualTo(SystemInfo.NullText), "Test no logical thread stack value set");
    stringAppender.Reset();

    using (LogicalThreadContext.Stacks[Utils.PropertyKey].Push("val1"))
    {
      log1.Info("TestMessage");
      Assert.That(stringAppender.GetString(), Is.EqualTo("val1"), "Test logical thread stack value set");
      stringAppender.Reset();

      using (LogicalThreadContext.Stacks[Utils.PropertyKey].Push(null))
      {
        log1.Info("TestMessage");
        Assert.That(stringAppender.GetString(), Is.EqualTo("val1 "), "Test logical thread stack value pushed null");
        stringAppender.Reset();
      }
    }

    log1.Info("TestMessage");
    Assert.That(stringAppender.GetString(), Is.EqualTo(SystemInfo.NullText), "Test logical thread stack value removed");
    stringAppender.Reset();
  }

  [Test]
  public async Task TestLogicalThreadStackPatternAsyncAwait()
  {
    StringAppender stringAppender = new()
    {
      Layout = new PatternLayout("%property{" + Utils.PropertyKey + "}")
    };

    ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
    BasicConfigurator.Configure(rep, stringAppender);

    ILog log1 = LogManager.GetLogger(rep.Name, "TestLogicalThreadStackPattern");

    log1.Info("TestMessage");
    Assert.That(stringAppender.GetString(), Is.EqualTo(SystemInfo.NullText), "Test no logical thread stack value set");
    stringAppender.Reset();

    string testValueForCurrentContext = "Outer";
    string[]? strings;
    using (LogicalThreadContext.Stacks[Utils.PropertyKey].Push(testValueForCurrentContext))
    {
      log1.Info("TestMessage");
      Assert.That(stringAppender.GetString(), Is.EqualTo(testValueForCurrentContext), "Test logical thread stack value set");
      stringAppender.Reset();

      strings = await Task.WhenAll(Enumerable.Range(0, 10).Select(x => SomeWorkStack(x.ToString()))).ConfigureAwait(false);
    }

    // strings should be ["Outer 0 AOuter 0 AOuter 0Outer 0 BOuter 0 B Outer 0", ...]
    for (int i = 0; i < strings.Length; i++)
    {
      Assert.That(strings[i], Is.EqualTo(string.Format("{0} {1} A{0} {1} A{0} {1}{0} {1} B{0} {1} B{0} {1}", testValueForCurrentContext, i)), "Test logical thread properties expected sequence");
    }

    log1.Info("TestMessage");
    Assert.That(stringAppender.GetString(), Is.EqualTo(SystemInfo.NullText), "Test logical thread properties value removed");
    stringAppender.Reset();
  }

  static async Task<string> SomeWorkProperties(string propertyName)
  {
    StringAppender stringAppender = new()
    {
      Layout = new PatternLayout("%property{" + Utils.PropertyKey + "}")
    };

    ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
    BasicConfigurator.Configure(rep, stringAppender);

    ILog log = LogManager.GetLogger(rep.Name, "TestLogicalThreadStackPattern");
    log.Info("TestMessage");

    // set a new one
    LogicalThreadContext.Properties[Utils.PropertyKey] = propertyName;
    log.Info("TestMessage");

    await MoreWorkProperties(log, "A").ConfigureAwait(false);
    log.Info("TestMessage");
    await MoreWorkProperties(log, "B").ConfigureAwait(false);
    log.Info("TestMessage");
    return stringAppender.GetString();
  }

  static async Task MoreWorkProperties(ILog log, string propertyName)
  {
    LogicalThreadContext.Properties[Utils.PropertyKey] = propertyName;
    log.Info("TestMessage");
    await Task.Delay(1).ConfigureAwait(false);
    log.Info("TestMessage");
  }

  private static async Task<string> SomeWorkStack(string stackName)
  {
    StringAppender stringAppender = new()
    {
      Layout = new PatternLayout("%property{" + Utils.PropertyKey + "}")
    };

    ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
    BasicConfigurator.Configure(rep, stringAppender);

    ILog log = LogManager.GetLogger(rep.Name, "TestLogicalThreadStackPattern");

    using (LogicalThreadContext.Stacks[Utils.PropertyKey].Push(stackName))
    {
      log.Info("TestMessage");
      Assert.That(stringAppender.GetString(), Is.EqualTo($"Outer {stackName}"), "Test logical thread stack value set");
      stringAppender.Reset();

      await MoreWorkStack(log, "A").ConfigureAwait(false);
      log.Info("TestMessage");
      await MoreWorkStack(log, "B").ConfigureAwait(false);
      log.Info("TestMessage");
    }

    return stringAppender.GetString();
  }

  static async Task MoreWorkStack(ILog log, string stackName)
  {
    using (LogicalThreadContext.Stacks[Utils.PropertyKey].Push(stackName))
    {
      log.Info("TestMessage");
      await Task.Delay(1).ConfigureAwait(false);
      log.Info("TestMessage");
    }
  }
}