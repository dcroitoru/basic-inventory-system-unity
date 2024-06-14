### UI Toolkit Inventory System
___

This package can be used to create various styles of inventories, ranging from RPGs to City Builders.

It features well known mechanics like main inventory, equipment, hotbar, vendor, chests.

As the name suggests, the inventory system makes use of UI Toolkit to render it's contents. If you are new to UI Toolkit, check out the "Getting Started" page [here](https://docs.unity3d.com/Manual/UIE-simple-ui-toolkit-workflow.html).


The project was built with a functional paradigm in mind, - you can use it to study some (hopefully) good FP practices.

The package includes a demo scene located at `Demos/Basic`. It shows all the mechanics currently supported by the system in the form an inventory window similar to the one from "Minecraft". 
You can move items around using pick and drop, 'buy' items from the shop, collect items from chests, consume items.

The system includes a main data store (`Systems/Store.cs`) where all the events are processed and new states are computed. All the components are loosely coupled (independent of one another) 
and can be easily changed or extended. When a new state is computed, events are dispatched and the UI updates itself based on the state slice it is subscribed to.

All the Custom Visual Element Components a stored in the `Views` folder, while styles and icons can be found in `Resources`. The custom components are built declaratively only using C# and no UXML. At this moment they cannot be used inside UI Builder.

The core of the system can be found in 3 files:
* **Types.cs** - includes only the types and no behavior
* **Factory.cs** - presents various ways of instantiating types, including some internal validation
* **Functions.cs** - implements all the behavior


### Adding new items
To add a new item to the items "Database", open `Inventory/DB.cs`, add a new value to `ItemType` enum, configure it's *class*, *icon*, and other aspects inside the same file.


### Changing behavior:
To change a particular behavior, edit the event listener corresponding to the event that interests you inside `Systems/Store.cs`. Use the callbacks for buying or collecting items as examples.


### Changing component style:
To change a component's style, follow the example of an existing component, edit it's USS file and add or change its icons or images inside `Resources` folder.