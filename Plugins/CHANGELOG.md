# Change Log

All notable changes to the "ScriptBee Default Plugin Bundle" will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- update Jint to 4.8.0

### Added 

- generate namespaces for C# Script Generator
- generate enums for C# Script Generator
- generate enums for Javascript Script Generator
- generate enums for Python Script Generator

## [2.0.0]

### Changed

- upgrade to dotnet 10
- upgrade DxWorks.ScriptBee.Plugin.Api to v2.0.0

## [1.1.0]

### Fixed

- Fixed Plugin Download issue for ScriptBee Default Plugin Bundle due to inconsistent naming of the version in the
  DxWorks Hub yaml and the tag creation
- Fixed ScriptBee Folder not created when navigating to the Plugins Tab
- Fixed ScriptBee Default Plugin not loading correctly even though it is installed

## [1.0.0]

### Added

- Initial release of default plugin bundle.
- Included standard HelperFunctions for script execution.
- Added default script runners for C#, Javascript, and Python via ScriptRunner.
