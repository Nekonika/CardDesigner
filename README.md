# CardDesigner

## Classes

### Template

| Name       | Required | Type        | Default   | Description                                                                                    |
| :--------- | :------: | :---------- | :-------- | :--------------------------------------------------------------------------------------------- |
| Width      |   [X]    | `integer`   |           | The with of the image you want to generate.                                                    |
| Height     |   [X]    | `integer`   |           | The height of the image you want to generate.                                                  |
| Background |   [ ]    | `string`    | #00000000 | The color that should be used for the background in HEX format. 4 Byte value - supports alpha. |
| Children   |   [ ]    | `Element[]` | []        | A collection of elements that should be drawn to this template.                                |

### Element

Elements must define a type. If the type must be specified, you will find the type name in the elements description.
For example: If you want to have an element of type `ShapeElement`, you must defined this value:
`"$type": "CardDesigner.Classes.ShapeElement, CardDesigner"`

| Name       | Required | Type        | Default                  | Description                                                                                    |
| :--------- | :------: | :---------- | :----------------------- | :--------------------------------------------------------------------------------------------- |
| Position   |   [ ]    | `IPosition` | `AbsolutPosition` (0, 0) | Where this element should be placed within its parent element.                                 |
| Background |   [ ]    | `string`    | #00000000                | The color that should be used for the background in HEX format. 4 Byte value - supports alpha. |
| Children   |   [ ]    | `Element[]` | []                       | A collection of elements that should be drawn within this element.                             |

### ImageElement : Element

This Element inherits from the base Element.
That means as much as that it can use the properties of Element aswell.
Type: `"$type": "CardDesigner.Classes.ImageElement, CardDesigner"`

| Name   | Required | Type      | Default   | Description                                                                   |
| :----- | :------: | :-------- | :-------- | :---------------------------------------------------------------------------- |
| Path   |   [X]    | `string`  |           | The path to the source of the image. This may be a local file path or an url. |
| Shape  |   [ ]    | `Shape`   | Rectangle | The shape of this element.                                                    |
| Width  |   [X]    | `integer` |           | The width of the image. Images will be stretched.                             |
| Height |   [X]    | `integer` |           | The height of the image. Images will be stretched.                            |

### ShapeElement : Element

This Element inherits from the base Element.
That means as much as that it can use the properties of Element aswell.
Type: `"$type": "CardDesigner.Classes.ShapeElement, CardDesigner"`

| Name            | Required | Type      | Default   | Description                                                                      |
| :-------------- | :------: | :-------- | :-------- | :------------------------------------------------------------------------------- |
| Shape           |   [ ]    | `Shape`   | Rectangle | The shape of this element.                                                       |
| Color           |   [ ]    | `string`  | #00000000 | The color of this elements border.                                               |
| BorderThickness |   [ ]    | `int`     | 0         | Defines how thick this elements border should be. A value of 0 hides the border. |
| Width           |   [X]    | `integer` |           | The width of the shape.                                                          |
| Height          |   [X]    | `integer` |           | The height of the shape.                                                         |

### TextElement : Element

This Element inherits from the base Element.
That means as much as that it can use the properties of Element aswell.
Type: `"$type": "CardDesigner.Classes.TextElement, CardDesigner"`

| Name          | Required | Type         | Default     | Description                                      |
| :------------ | :------: | :----------- | :---------- | :----------------------------------------------- |
| Content       |   [X]    | `string`     |             | The text you want to be displayed.               |
| FontFamily    |   [ ]    | `string`     | Arial       | The font that should be used for the text.       |
| FontSize      |   [ ]    | `int`        | 14          | The font size that should be used for the text.  |
| FontStyle     |   [ ]    | `FontStyle`  | Regular (0) | The font style that should be used for the text. |
| TextAlignment |   [ ]    | `AlignmentX` | Left        | Vertical text alignment.                         |
| Color         |   [ ]    | `string`     | #FFFFFF     | The color of this elements border.               |

### AbsolutPosition : IPosition

Type: `"$type": "CardDesigner.Classes.AbsolutPosition, CardDesigner"`

| Name | Required | Type      | Default | Description                                      |
| :--- | :------: | :-------- | :------ | :----------------------------------------------- |
| X    |   [X]    | `integer` |         | The X offset of this element whithin its parent. |
| Y    |   [X]    | `integer` |         | The Y offset of this element whithin its parent. |

### RelativePosition : IPosition

Type: `"$type": "CardDesigner.Classes.RelativePosition, CardDesigner"`

| Name       | Required | Type         | Default | Description                                                 |
| :--------- | :------: | :----------- | :------ | :---------------------------------------------------------- |
| AlignmentX |   [ ]    | `AlignmentX` | Left    | The horizontal alignment of this element within its parent. |
| AlignmentY |   [ ]    | `AlignmentY` | Top     | The vertical alignment of this element within its parent.   |


## Types

### AlignmentX

| Name   | Description                               |
| :----- | :---------------------------------------- |
| Left   | Aligns the element or text to the left.   |
| Center | Aligns the element or text to the center. |
| Right  | Aligns the element or text to the right.  |

### AlignmentY

| Name   | Description                       |
| :----- | :-------------------------------- |
| Top    | Aligns the element to the top.    |
| Middle | Aligns the element to the middle. |
| Bottom | Aligns the element to the bottom. |

### Shape

| Name      | Description                                                     |
| :-------- | :-------------------------------------------------------------- |
| Rectangle | Gives the element a rectangular shape.                          |
| Circle    | Gives the element a round shape. This may aswell be an elipsis. |

### FontStyle

You may use the name (`string`) for fields of this type.
If you want to combine styles, you will have to add their values together and use that value (`int`) instead of the name.
For example: If you want the text to be Bold and Underlined, you would have to add 1 and 4, which makes 5, so you would set this field to the value 5.

| Name      | Value (int) | Description                          |
| :-------- | :---------: | :----------------------------------- |
| Regular   |      0      | Normal text.                         |
| Bold      |      1      | Bold text.                           |
| Italic    |      2      | Italic text.                         |
| Underline |      4      | Underlined text.                     |
| Strikeout |      8      | Text with a line through the middle. |
