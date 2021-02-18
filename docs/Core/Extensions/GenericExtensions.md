# Generic Extensions
Generic Extensions usually target `System.Collection.Generic` namespace.
Extensions provided include:
1. [Move<T>(int oldIndex, int newIndex)](#movetint-oldindex-int-newindex)
2. [Move<T>(T item, int newIndex)](#movetint-oldindex-int-newindex)
3. [SearchPattern<T>(Func<T[], T, bool>[] matches)](#searchpatternfunc-t-t-bool-matches)
4. [Shuffle<T>()](#shufflet)
5. [Rotate<T>(int offset)](#rotatetint-offset)
6. [RotateLinquish<T>(int offset)](#rotatelinquishtint-offset)

Most of these extensions apply to `IList<T>` and the `SearchPattern<T>(Func<T[], T, bool>[] matches)` extension applies to all `IEnumerables`.

## `Move<T>(int oldIndex, int newIndex)`
This extension moves an item within the list from its current index(`oldIndex`)
to a new index(`newIndex`). The overload `Move<T>(T item, int newIndex)` moves the specified `item` from it's current index to the new index.

## `SearchPattern(Func T[], T, bool>[] matches)`
This extension searches an `IEnumerable<T>` for all items that match the provided search patterns.

## `Shuffle<T>()`
This extension shuffle's randomly, the items in a `IList` from their current order.

## `Rotate<T>(int offset)`
This extension rotates the items in a list by the provided `offset`. Consider a list with items A, B, C, D. If we rotate the list by, for example an offset of 1, then the new sequence of items in the list should be B,C,D,A.

Where the `offset` is a negative number, the list is rotated counter-clockwise.
For example, rotating the preceeding list by an offset of -1 will produce the result D, A, B, C.

Note that this is an O(n) operation, thus performance may be slow for exteremely large lists.

## `RotateLinquish<T>(int offset)`
Similar to `Rotate<T>(int offset)`. The major difference is that this method employs LINQ to rotate the items. Another difference is that, while the former can deal with negative offsets, this method __can only__ deal with positive offsets. 

Performance may be slightly better when employing this method. However, note that I have not personally tested whether the difference in perfromance is significant. 