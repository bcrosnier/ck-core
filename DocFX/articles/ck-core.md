---
uid: articles-ck-core
---
CK.Core
=======

The **CK.Core assembly** (NuGet package: [CK.Core](https://www.nuget.org/packages/CK.Core/))
contains core utility classes and interfaces, both static and non-static, used by other components.

> [!div class="alert alert-warning"]
> ***CK-Core*** (with a dash) is the name of the *complete set of components* containing CK.Core, CK.ActivityMonitor, CK.Monitoring, etc.<br>
> ***CK.Core*** (with a dot) is the name of *this particular assembly and package*.

Installing CK.Core
------------------

To install CK.Core, install the NuGet package [CK.Core](https://www.nuget.org/packages/CK.Core/) in your .NET library or application.

Compatibility
-------------

CK.Core is compatible with projects targeting the **.NET framework 4.5.1** or above.

Using CK.Core
-------------

This package contains a number of various utilities used in other components and libraries, grouped in coincidental cohesion, and does not come with one primary usage or purpose.

See below for the detailed contents of the package.

Contents
--------

### Configuration and application settings

#### [AppSettings](xref:CK.Core.AppSettings)
The [AppSettings](xref:CK.Core.AppSettings) class is a simple front to access AppSettings through <attr title="Dependency injection">DI</attr>-like calls,
with eg. an [AppSettings.Default](xref:CK.Core.AppSettings.Default) singleton and methods like [T GetRequired&lt;T&gt;(string key)](xref:CK.Core.AppSettings.GetRequired``1(System.String)).

### Binary reading/writing

#### [CKBinaryReader](xref:CK.Core.CKBinaryReader)
The [CKBinaryReader](xref:CK.Core.CKBinaryReader) class specializes [System.IO.BinaryReader](xref:System.IO.BinaryReader) with helper methods supporting nullable strings and compressed integers.

#### [CKBinaryWriter](xref:CK.Core.CKBinaryWriter)
The [CKBinaryWriter](xref:CK.Core.CKBinaryWriter) class specializes [System.IO.BinaryWriter](xref:System.IO.BinaryWriter) with helper methods supporting nullable strings and compressed integers.

### Exception handling and formatting

#### [CKException](xref:CK.Core.CKException)
The [CKException](xref:CK.Core.CKException) class specializes [System.Exception](xref:System.Exception) with serialization and helpers for text formatting and display.

#### [CKExceptionData](xref:CK.Core.CKExceptionData)
The [CKExceptionData](xref:CK.Core.CKExceptionData) class is an immutable, serializable representation of another [System.Exception](xref:System.Exception). Used by [CKException](xref:CK.Core.CKException).

#### [CriticalErrorCollector](xref:CK.Core.CriticalErrorCollector)

The [CriticalErrorCollector](xref:CK.Core.CriticalErrorCollector) class is a thread-safe collector of [System.Exception](xref:System.Exception) instances, raising events when errors are added.

### Collections

CK.Core defines the following collection-related interfaces:

- [ICKReadOnlyCollection&lt;T&gt;](xref:CK.Core.ICKReadOnlyCollection`1) - Contravariant interface inheriting [IReadOnlyCollection&lt;T&gt;](xref:System.Collections.Generic.IReadOnlyCollection`1)
- [ICKReadOnlyList&lt;T&gt;](xref:CK.Core.ICKReadOnlyList`1) - Covariant interface inheriting [IReadOnlyList&lt;T&gt;](xref:System.Collections.Generic.IReadOnlyList`1)
- [ICKReadOnlyUniqueKeyedCollection&lt;T, TKey&gt;](xref:CK.Core.ICKReadOnlyUniqueKeyedCollection`2) - Read-only, keyed collection of covariant items with a contravariant key
- [ICKReadOnlyMultiKeyedCollection&lt;T, TKey&gt;](xref:CK.Core.ICKReadOnlyMultiKeyedCollection`2) - Read-only, keyed collection of covariant items with a contravariant key, supporting multiple items having the same key
- [ICKWritableCollection&lt;T&gt;](xref:CK.Core.ICKWritableCollection`1) - Contravariant interface of a writable collection of items (with Add(), Remove(), Clear(), Count)
- [ICKWritableCollector&lt;T&gt;](xref:CK.Core.ICKWritableCollector`1) - Contravariant interface of an item collector (with Add(), Count)

#### [CKEnumeratorMono&lt;T&gt;](xref:CK.Core.CKEnumeratorMono`1)

The [CKEnumeratorMono&lt;T&gt;](xref:CK.Core.CKEnumeratorMono`1) class is an implementation of [IEnumerator&lt;T&gt;](xref:System.Collections.Generic.IEnumerator`1) containing and optimized for a single element.

#### [CKReadOnlyCollectionOnICollection&lt;T&gt;](xref:CK.Core.CKReadOnlyCollectionOnICollection`1)

The [CKReadOnlyCollectionOnICollection&lt;T&gt;](xref:CK.Core.CKReadOnlyCollectionOnICollection`1) class is an adapter class exposing an [IReadOnlyCollection&lt;T&gt;](xref:System.Collections.Generic.IReadOnlyCollection`1) from an [ICollection&lt;T&gt;](xref:System.Collections.Generic.ICollection`1).

#### [CKReadOnlyListOnIList&lt;T&gt;](xref:CK.Core.CKReadOnlyListOnIList`1)

The [CKReadOnlyListOnIList&lt;T&gt;](xref:CK.Core.CKReadOnlyListOnIList`1) class is an adapter class exposing an [IReadOnlyList&lt;T&gt;](xref:System.Collections.Generic.IReadOnlyList`1) from an [IList&lt;T&gt;](xref:System.Collections.Generic.IList`1).

#### [CKSortedArrayList&lt;T&gt;](xref:CK.Core.CKSortedArrayList`1)

The [CKSortedArrayList&lt;T&gt;](xref:CK.Core.CKSortedArrayList`1) class is a collection class keeping the order given by an [IComparer&lt;T&gt;](xref:System.Collections.Generic.IComparer`1).

#### [CKSortedArrayKeyList&lt;T, TKey&gt;](xref:CK.Core.CKSortedArrayKeyList`2)

The [CKSortedArrayKeyList&lt;T, TKey&gt;](xref:CK.Core.CKSortedArrayKeyList`2) class is a collection class keeping the order given by an [IComparer&lt;T&gt;](xref:System.Collections.Generic.IComparer`1), where the order is not given from the item itself.

#### [FIFOBuffer&lt;T&gt;](xref:CK.Core.FIFOBuffer`1)

The [FIFOBuffer&lt;T&gt;](xref:CK.Core.FIFOBuffer`1) class is a queue with a fixed maximum size. Its oldest items are dropped when its maximum size is reached.

#### Extensions

CK.Core defines the following collection-related extension classes in namespace `CK.Core`:

- [CKReadOnlyExtension](xref:CK.Core.CKReadOnlyExtension) 
- [CollectionExtension](xref:CK.Core.CollectionExtension) 
- [DictionaryExtension](xref:CK.Core.DictionaryExtension) 
- [EnumerableExtension](xref:CK.Core.EnumerableExtension) 

### Logging-related utilities (CKTrait, DateTimeStamp)

#### [CKTrait](xref:CK.Core.CKTrait)

The [CKTrait](xref:CK.Core.CKTrait) class is an immutable object associated to a string, which can be *atomic* (eg. "Alt"), or *combined* (eg. "Alt|Ctrl|Shift"). It is unique within a [CKTraitContext](xref:CK.Core.CKTraitContext).

Used as log entry tags in logging utilities, or when describing key combinations.

#### [CKTraitContext](xref:CK.Core.CKTraitContext)

The [CKTraitContext](xref:CK.Core.CKTraitContext) class is a context containing [CKTrait](xref:CK.Core.CKTrait) instances, used to register and make use of [CKTrait](xref:CK.Core.CKTrait) instances.

#### [SetOperation](xref:CK.Core.SetOperation)

The [SetOperation](xref:CK.Core.SetOperation) enum describes the various operations possible on a set of items (Union, Intersect, etc.). Can be used on eg. [CKTrait](xref:CK.Core.CKTrait) instances to perform combination operations.

#### [DateTimeStamp](xref:CK.Core.DateTimeStamp)

The [DateTimeStamp](xref:CK.Core.DateTimeStamp) class encapsulates a [DateTime](xref:System.DateTime) value with another value, unique per [DateTime](xref:System.DateTime) value, called a Uniquifier.

In logging, this safeguards the logging order for multiple log entries with the same [DateTime](xref:System.DateTime), giving the entry a unique key and making it potentially addressable.

CK.Core defines a [DateTimeStamp](xref:CK.Core.DateTimeStamp)-related extension class in namespace `CK.Core`: [DateTimeStampExtension](xref:CK.Core.DateTimeStampExtension)

### Files and input/output

#### [FileUtil](xref:CK.Core.FileUtil)

The [FileUtil](xref:CK.Core.FileUtil) static utility class contains file-related static properties and methods, such as:
- CompressFileToGzipFile(...) and CompressFileToGzipFileAsync(...)
- CheckForWriteAccess(...)
- CopyDirectory(...), supporting file filters
- NormalizePathSeparator(...)
- Utilities to create and make use of *uniquely-timed files*, when creating timestamped files (eg. logs), ensuring they can be created.

#### [TemporaryFile](xref:CK.Core.TemporaryFile)

The [TemporaryFile](xref:CK.Core.TemporaryFile) class is a disposable class, which creates a temporary file when instanciated, and deletes it when disposed.

### Access control

#### [GrantLevel](xref:CK.Core.GrantLevel)

The [GrantLevel](xref:CK.Core.GrantLevel) enum describes generic access levels of any actor on any resource. Intended for use in access control systems.

### Reflection and debugging

#### [SimpleTypeFinder](xref:CK.Core.SimpleTypeFinder)

The [SimpleTypeFinder](xref:CK.Core.SimpleTypeFinder) class is a static utility class to resolve [Type](xref:System.Type)s from their namespace and name, but without the complete [AssemblyQualifiedName](xref:System.Type.AssemblyQualifiedName) of their assembly.

### Dependency injection

#### [ISimpleServiceContainer](xref:CK.Core.ISimpleServiceContainer)

The [ISimpleServiceContainer](xref:CK.Core.ISimpleServiceContainer) interface describes a dependency injection container with the ability to add and remove services.

CK.Core defines an [ISimpleServiceContainer](xref:CK.Core.ISimpleServiceContainer)-related extension class in namespace `CK.Core`: [ServiceContainerExtension](xref:CK.Core.ServiceContainerExtension).

#### [SimpleServiceContainer](xref:CK.Core.SimpleServiceContainer)

The [SimpleServiceContainer](xref:CK.Core.SimpleServiceContainer) class is a disposable implementation of [ISimpleServiceContainer](xref:CK.Core.ISimpleServiceContainer).

### XML

CK.Core defines an XML-related extension class in namespace `CK.Core`: [XmlExtension](xref:CK.Core.XmlExtension).

### Miscellaneous interfaces and static utilities

#### [IMergeable](xref:CK.Core.IMergeable)

The [IMergeable](xref:CK.Core.IMergeable) interface describes an object supporting merging with another object.

#### [IUniqueId](xref:CK.Core.IUniqueId)

The [IUniqueId](xref:CK.Core.IUniqueId) interface describes an object having a unique [Guid](xref:System.Guid).

#### [Util](xref:CK.Core.Util)

The [Util](xref:CK.Core.Util) static utility class contains miscellaneous static methods and properties:

- Empty singletons for Enumerable, Array (pre-.NET 4.6), IDisposable, Action
- Binary search ([Wikipedia: Binary search algorithm](https://en.wikipedia.org/wiki/Binary_search_algorithm)) implementation methods
- Thread-safe (Interlocked) utility methods to perform Add, Remove, RemoveAll and Set operations on an Array
- Thread-safe (Interlocked) utility method to set a reference type to a variable

#### [Util.Hash](xref:CK.Core.Util.Hash)

The [Util.Hash](xref:CK.Core.Util.Hash) contains static utility methods to combine hash values.