using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SandS.Common.Pathes
{
    // ReSharper disable once AllowPublicClass
    public abstract class AbsolutePath : IComparable,
                                         IComparable<AbsolutePath>,
                                         IEquatable<AbsolutePath>
    {
        protected AbsolutePath(string rawPath)
        {
            var root = Path.GetPathRoot(rawPath);

            if (string.IsNullOrWhiteSpace(root))
            {
                throw new ArgumentException($"Absolute path is required, but the root for '{rawPath}' was not found");
            }

            RawPath = rawPath;
        }

        public static bool IsExistingDirectory(string path)
        {
            return Directory.Exists(path) && File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }

        public static bool IsExistingFile(string path)
        {
            return File.Exists(path) && !File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }

        public static implicit operator string(AbsolutePath absolutePath)
        {
            return absolutePath.RawPath;
        }

        public string RawPath { get; }

        public abstract string Name { get; }

        public abstract DirectoryPath Parent { get; }

        public abstract bool Exists { get; }

        public bool IsExistingDirectory()
        {
            return IsExistingDirectory(RawPath);
        }

        public bool IsExistingFile()
        {
            return IsExistingFile(RawPath);
        }

        public override string ToString()
        {
            return RawPath;
        }

#region comparer

        private static readonly AbsolutePathComparer LocalComparer = new AbsolutePathComparer();

        public IComparer<AbsolutePath> Comparer => LocalComparer;

        public IEqualityComparer<AbsolutePath> EqualityComparer => LocalComparer;

        private sealed class AbsolutePathComparer : IComparer,
                                                    IComparer<AbsolutePath>,
                                                    IEqualityComparer,
                                                    IEqualityComparer<AbsolutePath>
        {
            public int Compare(AbsolutePath lhs, AbsolutePath rhs)
            {
                const int equals = 0;
                const int notEquals = 1; // introduce less/greater constants if you need them

                if (lhs is null &&
                    rhs is null)
                {
                    return @equals;
                }

                if (lhs is null ||
                    rhs is null)
                {
                    return notEquals;
                }

                if (ReferenceEquals(lhs, rhs))
                {
                    return @equals;
                }

                var areEquals = string.Equals(lhs.RawPath, rhs.RawPath, StringComparison.InvariantCulture);

                return areEquals ? equals : notEquals;
            }

            public int Compare(object lhs, object rhs)
            {
                const int equals = 0;
                const int notEquals = 1;

                if (lhs is null &&
                    rhs is null)
                {
                    return @equals;
                }

                if (lhs is null ||
                    rhs is null)
                {
                    return notEquals;
                }

                if (ReferenceEquals(lhs, rhs))
                {
                    return @equals;
                }

                if (lhs is AbsolutePath lhsItem &&
                    rhs is AbsolutePath rhsItem)
                {
                    return Compare(lhsItem, rhsItem);
                }

                return notEquals;
            }

            public bool Equals(AbsolutePath lhs, AbsolutePath rhs)
            {
                return Compare(lhs, rhs) == 0;
            }

            public int GetHashCode(AbsolutePath item)
            {
                // Ensure that there are no warnings here
                // Ensure that there are no mutable fields in hash code calculations
                return item.RawPath.GetHashCode();
            }

            public int GetHashCode(object obj)
            {
                if (obj is AbsolutePath item)
                {
                    return GetHashCode(item);
                }

                return obj.GetHashCode();
            }

            bool IEqualityComparer.Equals(object lhs, object rhs)
            {
                return Compare(lhs, rhs) == 0;
            }
        }

        public int CompareTo(object obj)
        {
            return LocalComparer.Compare(this, obj);
        }

        public int CompareTo(AbsolutePath other)
        {
            return LocalComparer.Compare(this, other);
        }

        public override bool Equals(object obj)
        {
            return LocalComparer.Compare(this, obj) == 0;
        }

        public bool Equals(AbsolutePath other)
        {
            return LocalComparer.Compare(this, other) == 0;
        }

        public override int GetHashCode()
        {
            return EqualityComparer.GetHashCode(this);
        }

        public static bool operator ==(AbsolutePath lhs, AbsolutePath rhs)
        {
            return LocalComparer.Compare(lhs, rhs) == 0;
        }

        public static bool operator !=(AbsolutePath lhs, AbsolutePath rhs)
        {
            return LocalComparer.Compare(lhs, rhs) != 0;
        }

#endregion
    }
}