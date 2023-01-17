# Changelog

## [6.4.1] - 2021-10-13
 - Update legal docs with appropriate formatting.

## [6.4.0] - 2021-09-15
- Deprecating Lumin for 2021.1 and onwards.
- Fixed 90 degree offset issue for Image Tracking.
- Update the Image Tracking subsystem to create the Image Tracker disabled and synchronously.
- Explicitly add a way to increment the reference count of the Native Image Tracker to allow for external code to clean up the tracker.
- Emit XRInputSubsystem.trackingOriginUpdated event when Lumin reports a new tracking session.
- Fix a compiler warning about unused format arguments.
- Fixed a new issue with unity 2021.2.0a14 where building a lumin project gives an error (Fixes FB# 1328078)
- Fixed an issue where `isTracked` and `TrackingState` were not getting updated correctly (Fixes FB# 1175008)
- Addressed a case where getting the best controller may not return a valid controller.

## [6.2.2] - 2021-02-15
- Update C# assemblies to compile with "warnings as errors"
- Fix usage of obsolete APIs from XR Managment
- Fix an issue where eyetracking would fail in ZI.

## [6.2.0] - 2021-02-02
- Downgrade to AR Subsystems `4.0.12`

## [6.1.0] - 2021-01-28
- Bump minimum Unity version to `2020.2`
- Update XR Management to `4.0.1`
- Fixed a bug where setting 'Force Multipass' to true was being ignored.
- Fixed a case where hands weren't properly shutdown when stopping the XR Input subsystem

- Fixed a bug discovered with a later version of Visual Studio 2019.
- Fixed an issue where an unsupported MLSDK would prevent the Manifest Settings from rendering properly (Fixes FB# 1289174)
- Fixed a issue where image tracking would automatically activate the image tracking camera on the device.

## [6.1.0-preview.1] - 2020-08-26
- Fixed issue where enabling / disabling / re-enabling the `MagicLeapGestures` component would cause hands to lose tracking
for the rest of the session
- Fixed a minor issue where an invalid Coordinate Frame UID was incorrectly causing excessive log spam

## [6.0.1] - 2020-06-08
- Fixed issue where `MagicLeapGestureSubsystem` would spout a warning in editor when not targeting the Lumin Platform.

## [6.0.0] - 2020-05-28
- Update dependency for XR Management to `3.2.10`
- Fix issue where UnityMagicLeap libraries were included in Standalone Desktop builds when using Magic Leap
Zero Iteration Plugin Provider from XR Management (The intent of the Standalone Desktop provider is to allow
rapid iteration from the Unity Editor with Magic Leap's The Lab Zero Iteration module and not for use in standalone
builds)
- Updated dependency for ARSubsystems to `4.0.1`.

## [5.1.2] - 2020-04-07
- Conditionally compile out XR Management related classes that depend on XR Management `3.2.x`
- Revert dependency on XR Management `3.0.6`
- Add conditional code to support XR Management `3.2.X` workflows
- Conditionally compile samples to support lack of Legacy Input Helpers dependency.

## [5.1.1] - 2020-04-06
- Upgrade XR Management dependency to `3.2.4`

## [5.1.0] - 2020-03-16
- Upgrade XR Management dependency to 3.2.0 series
- Removed Legacy Input Helpers dependency as it is included implicitly through the XR Management 3.2.0 dependency
- APIs added for `QuerySupportedTrackingOriginModes`, `QueryTrackingOriginMode`, and `SetTrackingOriginMode`.
- Upgrade Lumin SDK to 0.23.0
- Fixed issue where `grip` and `gripButton` input usages did not surface proper data from the controller

## [5.0.1-preview.1] - 2020-02-06
- Add Image Tracking Subsystem
- Fix an issue preventing the gameview from showing when using ML Remote
- Remove old test assets that were conflicting with the Windows MR XR Plugin (Fixes FB #1200806)
- Properly handle a couple edge cases where the Lumin SDK is missing (Fixes FB #1201092)
- Fix a case where artifact dependencies were not properly available
- Fix Hand Tracking to no longer require a tracker restart on configuration changes
- Fix an issue with a meta file causing an issue with CI
- Fixed issue where using `NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray` resulted in an error while running in the Editor.

## [5.0.0] - 2019-11-20
- Renamed ReferencePointSubsystem to AnchorSubsystem
- Added support for plane classification
- Bump to 5.0.0 and update package dependencies
- Add custom MagicLeap layouts for the new input system
- Update repo to work as built-in test project
- Update NPM ignore list
- Fix some some bugs around the Display subsystem reinitializing
- Re-add a warning message on ML Remote when targeting an invalid graphics API
- Rework the ML manifest editor to be stabler and easier to use

## [4.0.4-preview.1] - 2019-09-03
- Fix an issue on OSX that caused ML Remote to intermittenly fail to initialize
- Improve the Input Provider Samples with some callbacks for handling button presses
- Add some additional checks around meshing shutdown

## [4.0.3] - 2019-08-21
- Update to 2019.3 Verified Release

## [4.0.3-preview.2] - 2019-08-20
- Source tests from the XR SDK Tests repo via a submodule
- Add some missing meta files back to the test directories
- Update package dependencies to verified version
- Fix an issue where the ML Remote check was wrong on OSX

## [4.0.3-preview.1] - 2019-08-13
- Update the Manifest editor to work with new trunk changes
- Fix Issue 1174014: Play in Editor in PC mode with Magic Leap loader and AR gestures will crash Unity Editor
- Add an explicit binary check when looking to launch ML Remote
- Allow for loading of gesture subsystem via XR Management

## [4.0.2] - 2019-08-01
- Update to 2019.3 Verified Release

## [4.0.2-preview.1] - 2019-07-31
- Make package name consistent with other XR provider plugins
- Add an additional define constraint for the rendering samples tests so they're not built by default
- Support both LIH 1.x and 2.x BasePoseProvider APIs
- Don't scale the near clipping plane
- Use updated clipping extents API function
- Add support for setting meshing density

## [4.0.1] - 2019-07-29
- Update to 2019.3 Verified Release

## [4.0.1-preview.1] - 2019-07-29
- Bump LIH to 1.3.4, which is the actual verified version in 2019.3
- Remove custom Input usages from the Experimental namespace
- Bump ARSubsystems to 2.1.0, which is the actual verified version in 2019.3
- Update XRDisplaySubstem to no longer use the Experimental namespace
- Update release field to 0a12, to indicate new required version

## [4.0.0] - 2019-07-17
- Initial 2019.3 Verified Release

## [4.0.0-preview.3] - 2019-07-16
- Update package description with note about disabled legacy XR
- Update XR Display provider header to latest from trunk
- Update Lumin SDK to 0.21.0
- Require Unity 2019.3.0a10 to ensure compatibility with XR Display headers
- Set the default frame timing hint to 60Hz
- Move StabilizationComponent into a Rendering sample

## [4.0.0-preview.2] - 2019-07-12
- Add some sample Base Pose Provider implementations based on XR Input
- Update XR Display provider header to latest from trunk
- Update XR Management to version 3

## [4.0.0-preview.1] - 2019-07-07
- Upgrade XR SDK to require Unity 2019.3
- Update package name
- Fix some issues preventing CI from completing successfully
- Update to latest version of XR Management package
- Update to latest version of Interaction Subsystems package

## [3.0.0-preview.7] - 2019-06-27
- Update documentation for 2019.2
- Robustify Meshing

## [3.0.0-preview.6] - 2019-06-20
- Fix an issue with meshing causing settings values to use garbage data
- Add support for determining the origin controller of a touchpas gesture event
- Fix a couple issues around proper handling of multiple controllers
- Properly support standalone subsystems that depend on the perception system
- Add support for standalone Planes, Raycast, and ReferencePoint subsystems
- Update Gestures documentation
- Fix a type collision with MagicLeap's Unity framework
- Add initial support for custom MagicLeap settings when using XR SDK
- Fix a couple issues that arise when using XR SDK, ML Remote, and repeatedly going in and out of playmode
- Bump Legacy Input Helpers to 1.3.2
- Fix an issue where timeouts from the ML Graphics API would cause the XR Display subsystem to shutdown
- Add support for multipass rendering on Lumin hardware and on ML Remote on Windows
- Fix an issue where XRSettings.renderViewportScale wasn't being propagated to ML's Graphics API

## [3.0.0-preview.5] - 2019-06-11
- Fix the native controller api loader to properly reference `ml_perception_client` instead of `ml_input`
- Fix an issue that prevented the Display provider from properly initializing in Editor using ML Remote
- Disable some old testing menu items
- Fix a couple cases where the UnityMagicLeap plugin would crash because it couldn't load the ML Remote libraries
- Add Multipass support for ML Remote on OSX
- Fix a bug where ML Remote / Zero Iteration on device would silently fail when using the XR SDK implementation
- Add some native support for managing controller feedback

## [3.0.0-preview.4] - 2019-05-20
- Update yamato configuration
- Improve how various ML input devices are handled via XR Input
- Simplify ML Remote library loading in the native plugin

## [3.0.0-preview.3] - 2019-05-18
- Update third party notices

## [3.0.0-preview.2] - 2019-05-17

## [3.0.0-preview.1] - 2019-05-17
- Add support for Unity 2019.2
- Add support for XR Display Subsystem
- Remove disabled clipping plane enforcement toggles
- Add support for hand tracking
- Add Manifest Editor UI
- Update package to build against 0.20.0 MLSDK
- Add support for starting / stopping ML Remote server headlessly via the Unity TestRunner
- Add standalone Gestures subsystem
- Do not fail when requesting confidence for a zero-vertex mesh
- Don't generate colliders for point cloud style meshes

## [2.0.0-preview.14] - 2019-03-05
- Initial Production release
- Fix a number of issues causing instabilty when using ML Remote
