# Models
Models are model classes provided by Core. Currently, core provides three models:

1. [ObservableQueue<T>](#observablequeue-and-stack)
2. [ObservalbeStack<T>](#observablequeue-and-stack)
3. [UniversalMessage](#universal-message)

## ObservableQueue and Stack
These are a `Queue` and a `Stack` that are "observable." This means that, like the `ObservableCollection` they implement `INotifyCollectionChanged` interface, which means that whenever there is an internal change in the collection, subscribes to the event will be notified. This is tipically used in Binding. Other than the implementation, they, at base behave `Queue` and `Stack`, meaning that you have methods like `ObservableQueue.Enqueue` and `ObservableStack.Pop`.

## Universal Message
The UniversalMessage is a model that allows you to model universal messages that you can transport accross apps that rely on Core in some fashion. Consider an example where you have a client application X and another Y, both of them doing drastically different things. Fortunately, however, they both use `Thismaker.Core`. Therefore, to facilitate communcication between them, you could use the UniveralMessage class as it will be understood by both programs. Note that this does not provide the message transport, to do that, you could perhaps look at `Thismaker.Mercury` that facilitates communication through sockets.
