using System;
using System.Collections.Generic;
using System.Linq;

using EnumerableExtension.Extensions;

using Xunit;

namespace EnumerableExtension.Tests
{
    public class Tests
    {
        public class EnumerableExtensionTest
        {
            private static Random Random { get; } = new Random();

            private class User
            {
                public User(string name)
                {
                    Name = name;
                    Id = Random.Next(10, 20);
                }

                public string Name { get; set; }

                public int Id { get; set; }
            }

            [Theory]
            [InlineData(10)]
            [InlineData(10000)]
            public void ShuffleMustWork(int count)
            {
                var arr = IEnumerableExtensions.Generate(count, () => Random.Next(-100, 100));
                var result = arr.Shuffle(Random);

                Assert.False(object.ReferenceEquals(arr, result));

                Assert.Equal(arr.Count(), result.Count());

                Assert.True(arr.SequenceEqualWithoutOrder(result));
                Assert.False(arr.SequenceEqual(result));
            }

            [Fact]
            public void IfDefaultGiveMeMustWorkForClass()
            {
                const string FirstUsername = "hello";
                const string SecondUsername = "another";

                {
                    var collection = new[] { new User(FirstUsername) };
                    var user = collection.FirstOrDefault().IfDefaultGiveMe(new User(SecondUsername));

                    Assert.Equal(FirstUsername, user.Name);
                }

                {
                    var anotherCollection = new User[0];
                    var user = anotherCollection.FirstOrDefault().IfDefaultGiveMe(new User(SecondUsername));

                    Assert.Equal(SecondUsername, user.Name);
                }
            }

            [Fact]
            public void IfDefaultGiveMeMustWorkForStruct()
            {
                {
                    var collection = new[] { 1 };
                    var item = collection.FirstOrDefault().IfDefaultGiveMe(2);

                    Assert.Equal(1, item);
                }

                {
                    var collection = new int[0];
                    var item = collection.FirstOrDefault().IfDefaultGiveMe(2);

                    Assert.Equal(2, item);
                }
            }

            [Fact]
            public void FirstOrAlternateMustWorkForStruct()
            {
                {
                    var collection = new[] { 1 };
                    var item = collection.FirstOrAlternate(2);

                    Assert.Equal(1, item);
                }

                {
                    var collection = new int[0];
                    int item = collection.FirstOrAlternate(2);

                    Assert.Equal(2, item);
                }
            }

            [Fact]
            public void FirstOrAlternateMustWorkForClass()
            {
                const string FirstUsername = "hello";
                const string SecondUsername = "another";

                {
                    var collection = new[] { new User(FirstUsername) };
                    var user = collection.FirstOrAlternate(new User(SecondUsername));

                    Assert.Equal(FirstUsername, user.Name);
                }

                {
                    var anotherCollection = new User[0];
                    var user = anotherCollection.FirstOrAlternate(new User(SecondUsername));

                    Assert.Equal(SecondUsername, user.Name);
                }
            }

            [Fact]
            public void ForEachWithFuncMustWork()
            {
                var array = new []{ 0, 1, 2, 3, 4, 5 };
                var result = array.ForEach(i => i * i).ToArray();

                for (var i = 0; i < array.Length; i++)
                {
                    Assert.Equal(array[i] * array[i], result[i]);
                    Assert.Equal(array[i], i);
                }

                Assert.Throws<ArgumentNullException>(() =>
                {
                    var arr = new[] { 1, 2, 3 };
                    Func<int, int> func = null;

                    arr.ForEach(func).IterateEnumerator();
                });
            }

            [Fact]
            public void ForEachWithActionMustWork()
            {
                var values = new List<int>();

                var array = new[] { 0, 1, 2, 3, 4, 5 };
                array.ForEach(i => values.Add(i)).IterateEnumerator();

                Assert.Equal(array.Length, values.Count);

                Assert.True(array.SequenceEqual(values));
                Assert.True(array.SequenceEqual(array));

                Assert.Throws<ArgumentNullException>(() =>
                {
                    var arr = new[] { 1, 2, 3 };
                    Action<int> action = null;

                    arr.ForEach(action).IterateEnumerator();
                });
            }
        }
    }
}
