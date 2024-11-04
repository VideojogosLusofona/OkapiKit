# Release Notes

## V1.16.1

- Fixed issue with SoundManager initialization of singleton

## V1.16.0

- Updated OkapiKit to Unity 6 (should work with previous versions)
- Added grid system
  - Unity Grid component can have a Grid System component, that allows for the Okapi Kit to use it for grid-based actions and movement.
  - Objects can now have a Grid Object component, that adds some functionality regarding grid based movement and actions like pushing
  - Most Movement components have a Grid counterpart now that acts as the original, but adapted for grid-based movement
  - New conditions that can test for Tiles or Tilesets at a specific position
  - New Change Tile action that allows users to changes tilemaps during runtime
- Improved Path UI in general
  - Numpad + now inserts points
  - Paths and points can be selected directly from the scene view
- Fixed some bugs with error reporting with animators
- Added animator control on MovementXY component

## V1.15.2

- 'Else' trigger on conditions and input will no longer throw an exception when an action is null

## V1.15.1

- Trigger will no longer throw an exception when an action is null
- Removed some GC from condition evaluation
- Renamed 'Box' mode on camera follow to 'Camera Trap'
- Added Exponential Decay camera mode, which is like Feedback Loop, but frame independent
- Added function to convert a path to world/local space, plus the ability to do that in multiple objects at the same time

## V1.15.0

- Added "any key" option to OnInput trigger

## V1.14.1

- Fixed issue with builds in version V1.14.0

## V1.14.0

- Added InputRotatesSprite and VelocityRotatesSprite to Flip Behaviour on platformer controller. Rotating instead of scaling changes the actual objects direction, so it can affect other movement systems, for example.
- Movement components can now have conditions built in which take care of activating/deactivating that movement
- Added Dash action, which enable short bursts of movement, using variables or values
- Improved display of title on Change Value action
- Probes now uses OkapiValues (can use numbers, or variables, etc)
- Probes now support using colliders for probe - can only detect intersections between active objects!
- Added Change Collider action, which allows to set the trigger option of a collider
- Fixed issue with Platformer movement - only non-trigger colliders can set the ground property
- Probes can now be identified by tag
- Follow path can now happen in normalized space, instead of constant speed
- Change transform action now can be scaled with time, can multiply/divide and can affect scaling as well as position
- Fixed bug with enums on editor
- Added snippets demonstrating a fuzzy aim system, and a alternate world game mechanic

## V1.13.0

- Added multi-variable text display (you can now use as a formatter something like "XP {0}/{1}", and use a MultiValueDisplayText component to provide multiple variables)
- Created structure to allow for in the future variables, literals or functions to be used as values for most things (internals)
- Added deprecated directories for scripts that will eventually be phased out

## V1.12.0

- Change Value action now can add/subtract a Variable/ValueHandler
- Change Value action can now set now assign to Variable/ValueHandler the value of another Variable/ValueHandler
- Change Value action can now multiply/dibvide, besides adding/subtracting
- Fixed a series of bugs with variable instances
- Comparisons can now include variables instead of just literal values
- Run Tagged Actions can now search on the object with which a collision just happened

## V1.11.3

- Fixed bug with coyote time enabling double jump

## V1.11.2

- Can now build games made with Okapi Kit
- Inertia on Movement_XY only appears when Input control is turned on
- Disabled colliders can't be used as spawn points
- SetParameter action is having issues detecting an animation parameter
- Fixed bug with selecting scene name when only one scene exists on Change Scene action
- Custom inspector for variables
- Custom inspector for hypertags
- Scale/Speed variance on spawner with more generous limits
- Blink now refers what the on/off duration do
- Blink throws warning with lack of renderer
- Target tag on Shake now disappears when an object is set
- Shake is now linked to target (through a Shaker behaviour), not to action itself - now we can destroy the object with the action when it's triggered
- Shake returns object to initial state
- Added warning when a button name is not defined (or doesn't exist)
- Added warning when destroy action is not the last action in the list
- Added warning when actions are not sorted properly

## V1.11.1

- Added access function to closed property on Path
- Fixed bug where spawn action uses prefab even when there's a spawner

## V1.11.0

- Added special values to SetAnimationParameter action - velocity.x/y, with abs
- Display spawn points on spawner, like we do on box areas or path
- Fixed small issues with display of tags
- Added initial delay on OnTimer trigger
- Changed how objects with rigid body are moved beneath the hood (shouldn't see any differences)
- Fixed bug with Absolute Velocity conditions, and added Velocity conditions
- Added ability to change FlipX/Y on ChangeSpriteRenderer action
- Some optimization of find by hypertag functions

## V1.10.1

- Fixed issue with world space path coordinates vs local space path coordinates
- Improved performance of Okapi UI elements significatly

## V1.10.0

- Added circle, arc and polygon path
- Modified path curve to use Catmull-Rom splines, it's a bit better
- Delete deletes point on path, while in edit mode and a point is selected
- Path can be closed
- Spawner now displays on the scene view the spawn area when it uses a box collider
- Spawner now can trigger how many cycles of spawn should it do by spawn action
- Spawner can now spawn objects along a path
- Added invert path option on path object
- Added center path option on path object
- Improved display and UI of path object
- Tag count condition can now be limited to a specific range
- Tag count with condition can now be rendered by in the scene view to be able to view the range (option on OkapiConfig scriptable object, enabled by default)
- Tags can now be seen in the scene view (option on OkapiConfig scriptable object, disabled by default)

## V1.9.2

- Fixed bug with ranges on ChangeMovement (changing velocity range would set the wrong values when X and Y had different ranges)

## V1.9.1

- Added else actions to OnInput
- Else actions on OnCondition weren't displaying warnings/errors

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
- Added tooltips to many of the debug/warning/error messages in the components, hover cursor for some hints
- Fixed issue with Display Text formatter with int/float type mismatch (for example, now {0:D6} works for having text with 6 digits, even if the number is a float - before it would just fail)
- Promoted platformer snippet to a full sample
- Separated Okapi Kit and the samples in two different packages
- Added Singleton system
  - Singletons are objects that enforce only one existing and that survive scene transitions
- Added ChangeSound action

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
