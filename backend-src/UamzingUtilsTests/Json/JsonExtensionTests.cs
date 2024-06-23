using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uamazing.Utils.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Uamazing.Utils.Json.Tests
{
    [TestClass()]
    public class JsonExtensionTests
    {
        [TestMethod()]
        public void PrimaryValue_For_NewtonJsonSerialize()
        {
            var value = true;
            var json = value.ToJson();
            Assert.IsTrue(json == "true");
        }
    }
}