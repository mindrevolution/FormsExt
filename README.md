# FormsExt for Umbraco Forms

FormsExt extends Umbraco Forms with Fieldtypes and Workflows, especially newsletter opt-in and marketing automation glue.


## Clone and build

> *You have been warned:* This package is a work in progress while I merge in features from several websites and internal packages made by mindrevolution.

* Umbraco 7.4+
* Umbraco Forms 4.1+
* Visual Studio 2015 (or above)


	#### build an Umbraco package in *../dist*
	```
	git clone https://github.com/mindrevolution/FormsExt.git
	cd FormsExt
	.\build.cmd

If you prefer to have a local test site and easy development, drop a current Umbraco release into */www*.


## Umbraco package (repository)

It's not there yet, but the build generates an Umbraco package already, so feel free to pick that up on the releases page.

> <https://github.com/mindrevolution/FormsExt/releases/new>


## Known Issues

Please be aware of the work in progress state

* Checkbox List (default Umbraco core)
* Image Cropper (default Umbraco core)
* Macro Container (default Umbraco core)
* Radiobutton List (default Umbraco core)
* Repeatable Textstring (default Umbraco core) - this works in the back-office, but due to a bug in the value-converter it will produce additional blank entries
* Upload (default Umbraco core)
* Dropdown List Multiple (default Umbraco core)

---

## Contributing to this project

Anyone and everyone is welcome to contribute. Until we have contribution guidelines in place, please [get in touch](#contact) so we can align things. Thanks!
<!--Please take a moment to review the [guidelines for contributing](CONTRIBUTING.md).-->
<!--* [Bug reports](CONTRIBUTING.md#bugs)
* [Feature requests](CONTRIBUTING.md#features)
* [Pull requests](CONTRIBUTING.md#pull-requests)-->


## <a name="contact"></a>Contact

* Twitter [@esn303](https://twitter.com/esn303)
* [Raise an issue](https://github.com/mindrevolution/FormsExt/issues) on GitHub


## License

Copyright &copy; 2017 [mindrevolution GmbH](https://mindrevolution.com) and [contributors](https://github.com/mindrevolution/FormsExt/graphs/contributors)

Licensed under the [MIT License](LICENSE.md)