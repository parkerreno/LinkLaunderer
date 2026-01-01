# iOS Share Extension Setup

This document explains the iOS Share Extension implementation for LinkLaunderer.

## Overview

The iOS share extension allows users to share URLs from other iOS apps directly to LinkLaunderer for cleaning and re-sharing, similar to the Android share action functionality.

## Implementation

The share extension consists of:

1. **ShareViewController.cs** - The main view controller that handles shared content
2. **Info.plist** - Extension configuration defining accepted content types
3. **Main App Info.plist** - Updated to support URL schemes

## How It Works

1. User shares a URL from another app (Safari, Twitter, etc.)
2. LinkLaunderer appears in the iOS share sheet
3. ShareViewController receives the shared URL
4. The URL is processed using LinkProcessor to clean it
5. The cleaned URL is presented back to the user via iOS share sheet
6. User can share the cleaned URL to their desired destination

## Build Configuration

**Note**: iOS Share Extensions in .NET MAUI require additional build configuration that may need to be done in Xcode on macOS.

### Current Status

The source files are in place:
- `/Platforms/iOS/ShareExtension/ShareViewController.cs`
- `/Platforms/iOS/ShareExtension/Info.plist`

The files are included in the csproj for iOS builds, but Share Extensions require being built as separate app extension bundles.

### Required Setup (On macOS)

When building on macOS, you may need to:

1. Open the project in Xcode (if necessary)
2. Add a Share Extension target to the iOS app
3. Replace the default extension code with the ShareViewController implementation
4. Ensure the extension's bundle identifier is set correctly (`com.companyname.linklaunderer.shareextension`)
5. Configure App Groups if the extension needs to share preferences with the main app

### Alternative: Using MSBuild Targets

A more integrated solution would involve creating custom MSBuild targets to automatically:
- Build the share extension as a separate bundle
- Include it in the app's PlugIns directory
- Sign it properly for distribution

This is an advanced setup that would require additional .targets files.

## Testing

To test the share extension on iOS:

1. Build and deploy the app to an iOS device or simulator (must be done on macOS)
2. Open Safari or another app
3. Navigate to a URL or select text containing a URL
4. Tap the Share button
5. Look for "LinkLaunderer" in the share sheet
6. Tap LinkLaunderer to process the URL
7. The cleaned URL should be presented for sharing

## Known Limitations

- Cannot be built or tested on Linux (iOS SDK required)
- Requires macOS with Xcode for proper extension bundling
- May require manual Xcode configuration for first-time setup
- App Group configuration needed if extension should share preferences with main app

## Future Improvements

- Create automated MSBuild targets for extension bundling
- Add App Groups support for sharing preferences
- Improve error handling and user feedback
- Add extension UI for showing processing status
