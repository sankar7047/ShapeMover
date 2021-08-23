# Shape Mover - Frostbite Melbourne WPF Coding Test

A WPF (C#) application that is a primitive graphics program. This program’s purpose is to create and manipulate a set of circular shapes shown on a canvas. Your program must:
- Provided a button that creates a new circle on the canvas at a random visible location,
- Allows the user to “drag” one of the circles to a new location on the canvas, and
- Provided buttons that perform undo and redo operations.
- Undoing creating a circle will delete that circle. Redoing it will make that appear at the point where it was initially created.
- Undoing moving a circle will return it to the position where it was at the beginning of the mouse operation. 
- Redoing it will make it appear at the location where it was at the end of the mouse operation.
- The undo (and redo) buttons will be disabled if there are no undo (or redo) operations that can be performed.

### The application has the followings:
- MVVM-based design
- Prism Framework
- A WPF Library has a custom panel to arrange the shapes

#### Example
![Example](https://user-images.githubusercontent.com/55825509/130463747-2400f625-ba64-48c8-8b17-e1896b5e7636.gif)

#### Application Output
![Application Output](https://user-images.githubusercontent.com/55825509/130464811-bf46db9f-9ad5-423b-a753-598719e526bd.png)

