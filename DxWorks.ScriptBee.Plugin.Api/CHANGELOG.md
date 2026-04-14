# Change Log

All notable changes to the "DxWorks.ScriptBee.Plugin.Api" project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- update Westwind.Utilities to 5.3.0

## [2.0.0]

### Changed

- upgrade to use dotnet 10
- update Westwind.Utilities to 5.2.8.1

## [1.2.0]

### Changed

- update Westwind.Utilities to 5.1.6.2

## [1.1.0]

### Changed

- Changed script runner plugin api to use parameters
- Added LoadModel method to IModelLoader plugin api to allow loading of models from stream with name
- Made LoadModel with only file stream list obsolete

## [1.0.0]

### Added

- Initial extraction of Plugin API interfaces (`IModelLoader`, `IModelLinker`, `IScriptRunner`, etc.).
- Establish core configuration properties and base models for plugins.

Added the following interfaces

- IPlugin
- IHelperFunctions
- IScriptGeneratorStrategy
- IScriptRunner
- IContext
- IProject
