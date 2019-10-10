# Narwhal .NET library

A .NET standard library to programatically load and save docker volumes from disk

# Getting Started

Install via .NET CLI
```bash
$ dotnet add package Narwhal.Lib
```

or 

Install via NuGet Package Manager
```powershell
PM> Install-Package Narwhal.Lib
```

# Usage
You can create a Narwhal instance to call methods to load and save volumes to tarballs. 

### Creating an instance 
The constructor boolean indicates whether you want the narwhal instance to be quiet.  
In non-quiet mode, it will print progress to console.
```c#
var narwhal = new Narwhal(false);
```

### Loading Tarball into a docker named volume
`Load(<volume name>, <path to tar>)`
```c#
narwhal.Load("my-volume","./data.tar.gz");
```

### Saving docker volume as a Tarball
`Save(<volume name>, <tarball name file name>, <relative path to save to>)`
```c#
narwhal.Save("my-volume", "data", "./");
```

## Contributing
Please read [CONTRIBUTING.md](CONTRIBUTING.MD) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning 
We use [SemVer](https://semver.org/) for versioning. For the versions available, see the tags on this repository.etting

## Authors
* [kirinnee](mailto:kirinnee97@gmail.com) 

## License
This project is licensed under MIT - see the [LICENSE.md](LICENSE.MD) file for details