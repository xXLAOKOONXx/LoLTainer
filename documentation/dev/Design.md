# Design

## Introduction

The application splits into multiple components linked together.

### IEventAPIManager
An Event API Manager manages a specific API that might trigger events.  
An example is the LCU API, which might trigger Events such as EnterChampionSelect.
A Event API Manager comes with some core functions:
- GetSupportedEvents: Returns all Events that can be triggered by the API
- GetEventHandler: Returns the EventListener that gets triggered when any Event accures from the API
- Connect: Connects to the API
- DisConnect: Disconnects from the API
- RestartCennection: Restarts the connection to the API
- SetActiveEvents: Receives a list of all used events. The API Manager might trigger different events based on which events are active
- Connected: Property to reflect the status whether the Manager is currently connected to the API or not

### IActionAPIManager
An Action API Manager manages a specific API that offers actions to be performed after an event.
An example is the SoundPlayer, which can play sound.
A Action API Manager comes with some core functions:
- PropertyList: readonly Dictionary with string as name of the Property and an enum type of the regarding Property
- ActionWindow: Window with an implementation of IActionWindow to edit a specific Action
- IsValidPropertyBundle: Receives a PropertyBundle and checks whether it is Valid
- PerformAtion: Receives a PropertyBundle and performs the Action as described in the PropertyBundle
- Connected: Property to reflect the status whether the Manager is currently connected to the API or not

### IActionWindow
An Window to create or edit an action.
The Window comes with core functions:
- Open: Receives a delegate to save, a delegate to cancel and a PropertyBundle and shows the Window to let the user costumize an action.

### PropertyBundle
A model class containing the following properties:
- ActionManager (enum)
- Properties (Dictionary: string propertyname, object value)

### EventActionSetting
A EventActionSetting bundles all events and their linked actions.
It consists of the following:
- Settings: Dictionary with event as enum and a list of PropertyBundle

### IApplicationManager
A ApplicationManager holds all API Managers and is responsable for the communication between them.
It has following core functions:
- IEnumerable of all Action API Managers
- IEnumerable of all Event API Managers
- EventActionSetting
- AllAvailableEvents: returns all Events currently available

### IFilterManager
A FilterManager holds the ability to give response on whether Filtercriterias are met or not.
It has following core functions:
- CheckFilter: recieves a FilterBundle