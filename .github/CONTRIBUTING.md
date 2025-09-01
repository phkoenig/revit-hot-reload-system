# 🤝 Contributing to Revit Hot Reload System

Thank you for your interest in contributing to this revolutionary Hot Reload system! 🚀

## 🎯 Project Vision

Our goal is to make Revit C# plugin development as fast and enjoyable as modern web development - with instant feedback and no more waiting for Revit to restart.

## 📋 How to Contribute

### 🐛 Reporting Bugs
- Use the [Bug Report template](.github/ISSUE_TEMPLATE/bug_report.md)
- Include detailed reproduction steps
- Attach log files from `%USERPROFILE%\Documents\GeoJsonImporter_HotReload.log`
- Test with a clean Revit installation if possible

### 💡 Suggesting Features
- Use the [Feature Request template](.github/ISSUE_TEMPLATE/feature_request.md)
- Explain the problem you're trying to solve
- Consider the impact on the overall architecture
- Think about compatibility with different Revit versions

### ❓ Asking Questions
- Use the [Question template](.github/ISSUE_TEMPLATE/question.md)
- Check existing documentation first
- Search previous issues and discussions

### 🔧 Code Contributions

#### Prerequisites
- **Revit 2026** installed
- **.NET 8.0** SDK
- **Visual Studio 2022** (recommended)
- **Git** and **GitHub CLI**
- Understanding of **AssemblyLoadContext** and **Reflection**

#### Development Setup
1. Fork the repository
2. Clone your fork:
   ```bash
   git clone https://github.com/YourUsername/revit-hot-reload-system.git
   cd revit-hot-reload-system
   ```
3. Build the project:
   ```bash
   dotnet build
   ```
4. Test the Hot Reload functionality in Revit

#### Code Style
- **C# Conventions**: Follow Microsoft's C# coding conventions
- **Comments**: Use XML documentation for public APIs
- **Logging**: Use `HotReloadLogger` for all logging
- **Error Handling**: Always include proper exception handling
- **Architecture**: Maintain separation between Loader-DLL and Work-DLL

#### Pull Request Process
1. **Create a branch** for your feature/fix:
   ```bash
   git checkout -b feature/amazing-improvement
   ```

2. **Make your changes** following our architecture:
   - **Loader-DLL changes**: Only if absolutely necessary
   - **Work-DLL changes**: For most plugin logic
   - **Documentation**: Update relevant docs

3. **Test thoroughly**:
   - Test the full Hot Reload cycle
   - Verify both Load and Unload work correctly
   - Test with different Revit scenarios
   - Check log output

4. **Commit with clear messages**:
   ```bash
   git commit -m "✨ Add feature: Auto-build on file change
   
   - Implement FileSystemWatcher for Work-DLL directory
   - Auto-trigger dotnet build when files change
   - Add configuration option to enable/disable
   - Update documentation with new workflow"
   ```

5. **Push and create PR**:
   ```bash
   git push origin feature/amazing-improvement
   ```
   Then create a Pull Request on GitHub

#### What We Look For
- **✅ Functionality**: Does it work as expected?
- **🏗️ Architecture**: Does it fit our Loader/Work-DLL pattern?
- **⚡ Performance**: Does it maintain fast Hot Reload times?
- **📚 Documentation**: Are changes documented?
- **🧪 Testing**: Can we verify it works?
- **🔄 Compatibility**: Works with Revit 2026+?

## 🏗️ Architecture Guidelines

### Core Principles
1. **Separation of Concerns**: Loader-DLL vs Work-DLL
2. **Clean Unloading**: AssemblyLoadContext must be collectible
3. **Dependency Isolation**: Custom dependency resolution
4. **Error Resilience**: Graceful handling of load/unload failures
5. **Developer Experience**: Simple, fast, reliable

### Key Components
- **WorkDllManager**: Core load/unload logic
- **Custom AssemblyLoadContext**: Dependency resolution
- **HotReloadLogger**: Centralized logging
- **UI Commands**: Simple Load/Unload buttons

### What NOT to Change
- **Core Architecture**: Loader/Work-DLL separation
- **AssemblyLoadContext Pattern**: isCollectible: true
- **File Locations**: WorkDll/ directory structure
- **Logging Interface**: HotReloadLogger methods

### Safe Areas for Changes
- **Work-DLL Logic**: Plugin implementation
- **UI Improvements**: Better buttons, feedback
- **Performance**: Faster load/unload cycles
- **Documentation**: Always welcome!
- **Examples**: More sample plugins

## 🧪 Testing Guidelines

### Manual Testing Checklist
- [ ] Build both Loader-DLL and Work-DLL successfully
- [ ] Start Revit - see 3 buttons in toolbar
- [ ] Load Work-DLL - success message appears
- [ ] Execute plugin - current version shows
- [ ] Unload Work-DLL - success message appears
- [ ] Build new Work-DLL version - no file locks
- [ ] Load Work-DLL - new version loads
- [ ] Execute plugin - new version shows
- [ ] Check log file - no errors

### Performance Testing
- Measure load/unload times (should be <1 second)
- Test with large Work-DLLs (>1MB)
- Test rapid load/unload cycles
- Monitor memory usage

## 📚 Documentation Standards

### When to Update Docs
- **New Features**: Always update relevant guides
- **API Changes**: Update technical specifications
- **Workflow Changes**: Update user guide
- **Architecture Changes**: Update architecture guide

### Documentation Files
- **README.md**: Overview and quick start
- **docs/README_HotReload_Architecture.md**: Technical deep-dive
- **docs/README_HotReload_UserGuide.md**: User workflows
- **docs/README_HotReload_TechnicalSpecs.md**: Specifications

## 🏆 Recognition

Contributors will be:
- **Listed in README.md** contributors section
- **Mentioned in release notes** for significant contributions
- **Credited in commit messages** with Co-authored-by
- **Invited as collaborators** for major contributors

## 📞 Getting Help

- **💬 GitHub Discussions**: General questions and ideas
- **🐛 GitHub Issues**: Specific bugs or features
- **📧 Direct Contact**: For sensitive issues

## 🙏 Code of Conduct

- **Be respectful** and inclusive
- **Be constructive** in feedback
- **Be patient** with newcomers
- **Be collaborative** not competitive
- **Have fun** building something amazing!

## 🚀 What's Next?

Check our [Project Roadmap](https://github.com/phkoenig/revit-hot-reload-system/discussions) for upcoming features and ways to contribute!

---

**Thank you for helping make Revit plugin development faster and more enjoyable for everyone! 🎉**

*This project could revolutionize how thousands of developers work with Revit - and you can be part of that revolution!*
