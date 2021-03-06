# Simple Graph Editor

A simple but quite useful implementation of a simple graphical editor. The sample node elements are inspired by logical elements. 

See the sample images for a quick impression of the UI.

## Features

Pre-defined elements can be dragged and dropped from a tool bar on to a work panel and afterwards shifted and deleted.

The elements own input (on the left side) and output connectors (on the right side). Using mouse click the connectors of two elements can be connected by a connection line. The connectors own labels that can be switched on an off.

When clicking on an element's connector the editor shows in yellow color the possible end connectors of the connection. This is implemented as a property of the underlying data object representing an element.

The connection lines can switch their display state thus displaying a red color to represent i.e. an 'activation' state. On the UI the two buttons 'Switch state' and 'Reset state' are used to simulate this feature.

The connection lines can be bent on random points to get a nicer routing. There is up to now no automatic routing implemented.

By default a grid is shown at which the elements snap to when dragged around. The grid can be switched off. Then the snapping is also inactive.

Some operations are possible via keyboard as well as via context menu (i.e. adding a new element on the work panel).

The elements deliver individual settings dialogs that are opened using context menu. Here the visibility of the connectors can be editied beside an individual element configuration.

The whole configuration can be saved and loaded to/from an XML file.

## Architecture

The elements (called `Node`) are defined as individual classes derived from a basic `NodeDataBase` class. The classes define the connector's look and name. Each of them own their own WPF UI where the settings tab is defined also.

The node types are added to `NodeTypeRepository` from where the elements are added dynamically to the tool bar in the main window and the context menu.

A static class is used to implement a primitive service container.

## Todos

- Automatic routing
- Using other component for the connection arrows than the one based on a `Shape`.
- Relocate the definition of a node and the connectors from code into an external (XML?) file.
