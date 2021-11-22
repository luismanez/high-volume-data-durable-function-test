using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HighVolumeDataFunctionApp.Models
{
    public abstract class Enumeration : IComparable
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        protected Enumeration() { }
        protected Enumeration(int id, string name) => (Id, Name) = (id, name);

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : Enumeration, new() =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                     .Select(f => f.GetValue(null))
                     .Cast<T>();

        public override bool Equals(object obj)
        {
            var otherEnumeration = obj as Enumeration;

            if (otherEnumeration == null)
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var idMatches = Id.Equals(otherEnumeration.Id);

            return typeMatches && idMatches;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static int AbsoluteDifference(Enumeration firstEnumeration, Enumeration secondEnumeration)
             => Math.Abs(firstEnumeration.Id - secondEnumeration.Id);

        public static T ParseFromId<T>(int id) where T : Enumeration, new()
            => Parse<T, int>(id, "id", item => item.Id == id);

        public static T ParseFromName<T>(string name) where T : Enumeration, new()
            => Parse<T, string>(name, "name", item => item.Name.ToLower() == name.ToLower());

        private static T Parse<T, K>(K id, string description, Func<T, bool> predicate) where T : Enumeration, new()
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
            {
                var message = string.Format("'{0}' is not a valid {1} in {2}", id, description, typeof(T));
                throw new ApplicationException(message);
            }

            return matchingItem;
        }

        public static bool TryParseFromId<T>(int id, out T matchingItem) where T : Enumeration, new()
            => TryParse<T>(item => item.Id == id, out matchingItem);
        public static bool TryParseFromId<T>(int id) where T : Enumeration, new()
            => TryParse<T>(item => item.Id == id);

        public static bool TryParseFromName<T>(string name, bool ignoreCase, out T matchingItem) where T : Enumeration, new()
            => TryParse<T>(item =>
            ignoreCase ? item.Name.ToLower() == name.ToLower() : item.Name == name, out matchingItem);
        public static bool TryParseFromName<T>(string name, bool ignoreCase) where T : Enumeration, new()
            => TryParse<T>(item =>
            ignoreCase ? item.Name.ToLower() == name.ToLower() : item.Name == name);

        public static bool TryParseFromIdOrName<T>(string idOrName, bool ignoreCase, out T matchingItem) where T : Enumeration, new()
            => TryParse<T>(item =>
            ignoreCase ? item.Name.ToLower() == idOrName.ToLower() : item.Name == idOrName, out matchingItem) ||
            int.TryParse(idOrName, out var id)
                && TryParse<T>(item => item.Id == id, out matchingItem);
        public static bool TryParseFromIdOrName<T>(string idOrName, bool ignoreCase) where T : Enumeration, new()
            => TryParse<T>(item =>
            ignoreCase ? item.Name.ToLower() == idOrName.ToLower() : item.Name == idOrName) ||
            int.TryParse(idOrName, out var id)
                && TryParse<T>(item => item.Id == id);

        private static bool TryParse<T>(Func<T, bool> predicate, out T matchingItem) where T : Enumeration, new()
        {
            matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
            {
                matchingItem = GetAll<T>().FirstOrDefault();
                return false;
            }

            return true;
        }
        private static bool TryParse<T>(Func<T, bool> predicate) where T : Enumeration, new()
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
            {
                return false;
            }

            return true;
        }

        public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
    }
}
