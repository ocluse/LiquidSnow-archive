# ObjectModel Extensions
These extensions apply technically for the `ObservableCollection<T>` to allow it to function more like the `List<T>` we know and love.

Provided extensions include:
1. [AddRange<T>(IEnumerabe<T> list)](#addrangetienumerablet-list)
2. [RemoveAll<T>(IEnumerable<T> list)](#removealltienumerablet-list)
3. [FindAll<T>(Func<T, bool>predicate)](#findtfunct-bool-predicate)
4. [Find<T>(Func<T, bool> predicate)](#findtfunct-bool-predicate)
5. [Exists<T>(Fun<T, bool>predicate)](#existstfunct-bool-predicate)
6. [Sort<T>(Comparison<T> comparison>)](#sorttcomparisont-comparison)

## `AddRange<T>(IEnumerable<T> list)`
This extensions adds all the items from `IEnumerable<T> list` into the ObservableCollection. Note that then `ICollectionChanged` event will be fired for every item that is added to the collection.

## `RemoveAll<T>(<IEnumerable<T> list>)`
This extension removes all items included in the `IEnumerable<T> list` that occur in the ObbservableCollection. Note that then `ICollectionChanged` event will be fired for every item that is removed to the collection.

## `Find<T>(Func<T, bool> predicate)`
This extension attempts to find an item that fits the provided `predicate`. This is similar to `List.Find()` that usually returns the first element in the sequence that matches the provided pattern.

The sister operation `FindAll<T>(Func<T, bool> predicate)` finds all the items that match the provided predicate.

## `Exists<T>(Func<T, bool> predicate)`
This extension can be used to determine there is any item in the ObservableCollection that matches the provided predicate.

## `Sort<T>(Comparison<T> comparison)`
Sorts the items in an ObservableCollection following the provided comparison. If no compraison is provided, then the items are sorted according to the default comparer of the `T` provided. For example, `string` may be sorted alphabetically, while  `int` will be sorted from smallest to largest.

The advantage of using this function is that `ICollectionChanged` event is always invoked, and thus where binding was used, the BindingTarget will always share the same order of items as that in the ObservableCollection. This can be used to provide powerful sorting options to end users.