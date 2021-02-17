# Introduction
These extensions apply technically for the `ObservableCollection<T>` to allow it to function more like the `List<T>` we know and love.

Provided extensions include:
1. AddRange<T>(IEnumerabe<T> list)
2. RemoveAll(IEnumerable<T> list)
3. FindAll(Func<T, bool>predicate)
4. Find<T>(Func<T, bool>predicate)
5. Exists<T>(Fun<T, bool>predicate)
6. Sort<Comparison<T> comparison> 

# Extensions
## `AddRange<T>(IEnumerable<T> list)`
This extensions adds all the items from `IEnumerable<T> list` into the ObservableCollection.

## `RemoveAll<IEnumerable<T> list>`
This extension removes all items included in the `IEnumerable<T> list` that occur in the ObbservableCollection.

## `Find<T>(Func<T, bool> predicate)`
This extension attempts to find an item that fits the provided `predicate`. This is similar to `List.Find()` that usually returns the first element in the sequence that matches the provided pattern.

