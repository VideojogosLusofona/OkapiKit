# Release Notes

## V1.9.0

- Added target tag and target object direction option to probe, to allow for maximum distance
- Fixed bug with uninitialized actions when added to triggers throwing an error
- Added Rotate Towards action
- Added warning/error system
  - Placing the cursor on a log message will sometimes provide a more detailed explanation.
- Renamed all ActionModify* actions to ActionChange*
- Added target to ActionChangeRigidBody (only allowed self object)
- Renamed ActionChangeSystem to ActionChangeSystemOption
- Fixed bug with Unity event UI on ActionUnityEvent
- MovementPlatformer now shows animator property
- Added ping system for actions (right-click on action in triggers, sequences and random
  and that will select the object that contains the action and highlight its title bar) - Ping only works if there's a Okapi Config instance in the project
- Added tooltips to all Okapi elements

## V1.8.0

- Added category for OkapiKit under the "Add Component" dialog
- Improved Camera Follow 2d script, with support for following multiple objects (through tags), averaging their position, and zooming in/out to keep them on screen.

## V1.7.2

- Set GUIUtils as a public class

## V1.7.1

- Changed options on editor asmdef

## V1.7.0

- Added some extensions to hypertag handling
- Fixed some bugs
- You can now make builds with this version

## V1.6.0

- Added inertia controls to MovementXY

## V1.5.1

- Added Probe to hierarchy display
- Improved tooltip display
- Added movement tooltip

## V1.5.0

- Added hierarchy view icons for all Okapi items that an object has
- Added tooltips when those icons are hovered

![Hierarchy](Screenshots/hierarchy01.png)

## V1.4.0

- Added Platform Movement system, to easily build platform games
- Added conditions that use the Platform Movement (IsGrounded, IsGliding)
- Added properties to change on Action Change Movement.

## V1.3.3

- Changes were due to allowing Okapi Kit to be used from the Unity Package Manager. You can add it to your projects now by adding a package from the git URL: "https://github.com/VideojogosLusofona/OkapiKit.git#upm".

## V1.3.2

- Removed hypertag drag and drop that created the HypertaggedObject component automatically, it conflicted with the other drag&drop operations on the components (previous release incorrectly stated this fix, but it wasn't commited properly)
- Removed drag and drop of actions directly, without instanting them first on the object
- Fixed bug with tag count

## V1.3.0

- Fixed bug with Follow Movement not respecting properly the "relative movement" flag
- Added Probe component, which allows for raycasts to be performed and used in conditions
- Added stop distance option on Follow Movement
- Added condition negation option
- Added "Else" clause for condition trigger, which is run if the condition is false
- Action and Condition arrays no longer state "Element x" in them, adding space
- Actions can now be dragged directly to the Actions element of triggers, without creating one first
- Hypertags now can be dropped directly in the Inspector and the tag is added to the HypertaggedObject component. If the component doesn't exist, it gets created.

## V1.2.0

- Fixed bug with inactive triggers still being triggered (onTimer, for example)
- Added UI element for tagged actions
- Allowed for search parameter on the Tagged action to search within one or more colliders (for example, good to make explosions affect an area)

## V1.1.1

- Fixed bugs with Blink action (it now only triggers when actually triggered)
- Actions now initialize in Awake, to guarantee that OnStart trigger can actually trigger the actions
- OnInput now shows cooldown property always (which matches code behaviour)
- Fixed more issues with icon caching

## V1.1.0

- Common class to all Okapi systems (OkapiElement)
  - Added common code to Editor (all objects derive from it)
- Removed TriggerRandom
- There are now special icons on all Okapi scripts
- Replace code icons with actual icons (polutes images, but it's faster and less error prone)
- Rotate Towards Movement Direction implemented (on MovementRotate component)
- Probability on Action Random equal to zero no longer shows wrong text
- Fixed some text getting wrapped instead of clipped on titles of text boxes
- Changed references to "self" to "this"
- Refresh on OnTriggerReset and OnTriggerInput is now working properly

## V1.0.0

- Initial version
