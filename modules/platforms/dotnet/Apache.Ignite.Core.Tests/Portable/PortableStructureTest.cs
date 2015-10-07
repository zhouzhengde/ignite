﻿/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Apache.Ignite.Core.Tests.Portable
{
    using System;
    using System.Collections.Generic;
    using Apache.Ignite.Core.Impl;
    using Apache.Ignite.Core.Impl.Portable;
    using Apache.Ignite.Core.Impl.Portable.Structure;
    using Apache.Ignite.Core.Portable;
    using NUnit.Framework;

    /// <summary>
    /// Contains tests for portable type structure.
    /// </summary>
    [TestFixture]
    public class PortableStructureTest
    {
        /** Repeat count. */
        public static readonly int RepeatCnt = 10;

        /** Objects per mode. */
        public static readonly int ObjectsPerMode = 5;

        /// <summary>
        /// Test object write with different structures.
        /// </summary>
        [Test]
        public void TestStructure()
        {
            for (int i = 1; i <= RepeatCnt; i++)
            {
                Console.WriteLine(">>> Iteration started: " + i);

                // 1. Generate and shuffle objects.
                IList<BranchedType> objs = new List<BranchedType>();

                for (int j = 0; j < 6 * ObjectsPerMode; j++)
                    objs.Add(new BranchedType((j%6) + 1));

                objs = IgniteUtils.Shuffle(objs);

                // 2. Create new marshaller.
                PortableTypeConfiguration typeCfg = new PortableTypeConfiguration(typeof(BranchedType));

                PortableConfiguration cfg = new PortableConfiguration
                {
                    TypeConfigurations = new List<PortableTypeConfiguration> { typeCfg }
                };

                PortableMarshaller marsh = new PortableMarshaller(cfg);

                // 3. Marshal all data and ensure deserialized object is fine.
                foreach (BranchedType obj in objs)
                {
                    Console.WriteLine(">>> Write object [mode=" + obj.mode + ']');

                    byte[] data = marsh.Marshal(obj);

                    BranchedType other = marsh.Unmarshal<BranchedType>(data);

                    Assert.IsTrue(obj.Equals(other));
                }
                
                Console.WriteLine();

                // 4. Ensure that all fields are recorded.
                IPortableTypeDescriptor desc = marsh.Descriptor(typeof (BranchedType));

                PortableStructure typeStruct = desc.TypeStructure;

                IDictionary<string, byte> fields = typeStruct.FieldTypes;

                Assert.IsTrue(fields.Count == 8);

                Assert.IsTrue(fields.ContainsKey("mode"));
                Assert.IsTrue(fields.ContainsKey("f2"));
                Assert.IsTrue(fields.ContainsKey("f3"));
                Assert.IsTrue(fields.ContainsKey("f4"));
                Assert.IsTrue(fields.ContainsKey("f5"));
                Assert.IsTrue(fields.ContainsKey("f6"));
                Assert.IsTrue(fields.ContainsKey("f7"));
                Assert.IsTrue(fields.ContainsKey("f8"));
            }
        }
    }

    public class BranchedType : IPortableMarshalAware
    {
        public int mode;
        public int f2;
        public int f3;
        public int f4;
        public int f5;
        public int f6;
        public int f7;
        public int f8;

        public BranchedType(int mode)
        {
            this.mode = mode;

            switch (mode)
            {
                case 1:
                    f2 = 2;

                    break;

                case 2:
                    f2 = 2;
                    f3 = 3;
                    f4 = 4;

                    break;

                case 3:
                    f2 = 2;
                    f3 = 3;
                    f5 = 5;

                    break;

                case 4:
                    f2 = 2;
                    f3 = 3;
                    f5 = 5;
                    f6 = 6;

                    break;

                case 5:
                    f2 = 2;
                    f3 = 3;
                    f7 = 7;

                    break;

                case 6:
                    f8 = 8;

                    break;
            }
        }

        public void WritePortable(IPortableWriter writer)
        {
            writer.WriteInt("mode", mode);

            switch (mode)
            {
                case 1:
                    writer.WriteInt("f2", f2);

                    break;

                case 2:
                    writer.WriteInt("f2", f2);
                    writer.WriteInt("f3", f3);
                    writer.WriteInt("f4", f4);

                    break;

                case 3:
                    writer.WriteInt("f2", f2);
                    writer.WriteInt("f3", f3);
                    writer.WriteInt("f5", f5);

                    break;

                case 4:
                    writer.WriteInt("f2", f2);
                    writer.WriteInt("f3", f3);
                    writer.WriteInt("f5", f5);
                    writer.WriteInt("f6", f6);

                    break;

                case 5:
                    writer.WriteInt("f2", f2);
                    writer.WriteInt("f3", f3);
                    writer.WriteInt("f7", f7);

                    break;

                case 6:
                    writer.WriteInt("f8", f8);

                    break;
            }
        }

        public void ReadPortable(IPortableReader reader)
        {
            mode = reader.ReadInt("mode");

            switch (mode)
            {
                case 1:
                    f2 = reader.ReadInt("f2");

                    break;

                case 2:
                    f2 = reader.ReadInt("f2");
                    f3 = reader.ReadInt("f3");
                    f4 = reader.ReadInt("f4");

                    break;

                case 3:
                    f2 = reader.ReadInt("f2");
                    f3 = reader.ReadInt("f3");
                    f5 = reader.ReadInt("f5");

                    break;

                case 4:
                    f2 = reader.ReadInt("f2");
                    f3 = reader.ReadInt("f3");
                    f5 = reader.ReadInt("f5");
                    f6 = reader.ReadInt("f6");

                    break;

                case 5:
                    f2 = reader.ReadInt("f2");
                    f3 = reader.ReadInt("f3");
                    f7 = reader.ReadInt("f7");

                    break;

                case 6:
                    f8 = reader.ReadInt("f8");

                    break;
            }
        }

        public bool Equals(BranchedType other)
        {
            return mode == other.mode && f2 == other.f2 && f3 == other.f3 && f4 == other.f4 && f5 == other.f5 &&
                   f6 == other.f6 && f7 == other.f7 && f8 == other.f8;
        }
    }
}
