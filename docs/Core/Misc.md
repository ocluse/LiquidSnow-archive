# Other Solutions provided by Core
Core has two further classes, these are `BindableBase` and `DeltaTime`.

## BindableBase
This is an essential class to anyone engaging in binding of any form. Inheriting from this class allows you to make you Models binding ready. Binding is a technique where you abstract the holders of data from it's representers. You can check the docs by Microsoft for a good introduction to binding and why it is important.

For usage example, imagine you have a class called `Contact` that has properties for Name, Email and Phone, as shown:

```cs
class Contact
{
    public string Name {get; set;}
    public string Email {get; set;}
    public string Phone {get; set;}
}
```
as you can see, this class is not binding ready, meaning that if you were to use it in, for example, a WPF application, any changes to name, email or phone would not be reflected on the UI. However to make it binding ready, you can use `BindableBase` as shown bellow:

```cs
class Contact : BindableBase
{
    private string _name, _email, _phone;
    
    public string Name
    {
        get {return _name;}
        set {SetProperty(ref _name, value);}
    }

    public string Email
    {
        get {return _email;}
        set {SetProperty(ref _email, value);}
    }

    public string Phone
    {
        get{return _phone;}
        set{SetProperty(ref _phone, value)}
    }
}
```
With this modification, the `Contact` class becomes bindable, i.e it implements the `INotifyPropertyChanged` interface that has the `PropertyChanged` event. This means that when a value is changed, for example, `Email`, subscribes to the `PropertyChanged` event will be notified with the the calling object instance, and the name of the property that has changed.

Note the `SetProperty(ref object source, object value)` method. Using this method, one assigns the value of `value` to `source` and in the process invokes the `PropertyChanged` event. Note that where the value of `source` is similar, according to the default comparer, to the value of `value` then no assignment will be made, thus the `PropertyChanged` event is not fired. This means that the event is only invoked when the property has really changed.

Uses of classes that implement `INotifyPropertyChanged` are extensive and go beyond simple binding. You could maybe desire to invoke a chain of events when a certain value has been changed, for example, auto-submitting a form once all the provided values have been input and check out. To do this, you can subscribe to the property changed event of the form, and on every change validate the values. If all check out, then you auto-submit.

See the example below for how to subscibe to `BindableBase`:
```cs
static void Main(string[] args)
{
    var contact = new Contact();
    contact.PropertyChanged += (o,e) =>
    {
        //o=>the sending object, in this case contact
        //e=>the EventArgs, containing the property name of changed event.
        Console.WriteLine("Contact Changed");

        //Using the object.GetPropValue extension provided by core, you can:
        Console.WriteLine("New property value: "+ contact.GetPropValue<string>(e.PropertyName))
    }

    //Do something that changes a value in contact. e.g
    contact.Name=Console.ReadLine();
}
```
The preceeding example makes use of the `GetPropValue(string)` extension provided by Core. To see more extensions provided by core, see [Extensions](.\Extensions\Extensions.md)