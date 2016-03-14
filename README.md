<p align="center">
<img src="https://xamarin.com/content/images/pages/branding/assets/xamarin-logo.png" alt="Xamarin" title="Xamarin" width="500">
</p>
<p align="center" >
<img src="http://oi60.tinypic.com/116jd51.jpg" alt="Particle" title="Particle">
</p>

# Particle Xamarin Cloud SDK
[![license](https://img.shields.io/hexpm/l/plug.svg)](https://github.com/spark/spark-sdk-ios/blob/master/LICENSE) 

Particle Xamarin Cloud SDK enables Xamarin apps to interact with Particle-powered connected products via the Particle Cloud. It’s an easy-to-use wrapper for Particle REST API. The Cloud SDK will allow you to:

- Manage user sessions for the Particle Cloud (access tokens, encrypted session management)
- Claim/Unclaim devices for a user account
- Get a list of instances of user's Particle devices
- Read variables from devices
- Invoke functions on devices
- Publish events from the mobile app and subscribe to events coming from devices *(New!)*

All cloud operations take place asynchronously and use the well-known completion blocks (closures for swift) design pattern for reporting results allowing you to build beautiful responsive apps for your Particle products and projects.
iOS Cloud SDK is implemented as an open-source Cocoapod static library and also as Carthage dynamic framework dependancy. See [Installation](#installation) section for more details. 

**Beta notice**

This SDK is still under development and is currently released as Beta. Although tested, bugs and issues may be present. Some code might require cleanup. In addition, until version 1.0 is released, we cannot guarantee that API calls will not break from one Cloud SDK version to the next. Be sure to consult the [Change Log](https://github.com/spark/spark-sdk-ios/blob/master/CHANGELOG.md) for any breaking changes / additions to the SDK.

## Getting Started

- Fork this library to get access to the SDK and a full working sample

## Usage

Cloud SDK usage involves two basic classes: first is `ParticleCloud` which is a singleton object that enables all basic cloud operations such as user authentication, device listing, claiming etc. Second class is `ParticleDevice` which is an instance representing a claimed device in the current user session. Each object enables device-specific operation such as: getting its info, invoking functions and reading variables from it.

### Common tasks

Here are few examples for the most common use cases to get your started:

#### Logging in to Particle cloud
You don't need to worry about access tokens, SDK takes care of that for you

```var loginSuccess = await ParticleCloud.SharedInstance.LoginWithUserAsync("username@email.com"","password");
if(loginSuccess)
    System.DIagnostics.Debug.WriteLine("Logged in to cloud")
else
    System.DIagnostics.Debug.WriteLine("Wrong credentials or no internet connectivity, please try again")```

#### Get a list of all devices
List the devices that belong to currently logged in user and find a specific device by name:

```C#
var devices = await ParticleCloud.SharedInstance.GetDevicesAsync();
```

#### Read a variable from a Particle device (Core/Photon/Electron)
Assuming here that `myPhoton` is an active instance of `SparkDevice` class which represents a device claimed to current user:
 
```C#
var temperature = await ParticleDevice.GetVariableAsync("temperature");
```

#### Call a function on a Particle device (Core/Photon/Electron)
Invoke a function on the device and pass a list of parameters to it, `resultCode` on the completion block will represent the returned result code of the function on the device.
This example also demonstrates usage of the new `NSURLSessionDataTask` object returned from every SDK function call.

```C#
var response = await ParticleDevice.CallFunctionAsync("lightShow");
```
#### Get an instance of a device
Get a device instance by its ID:
   
```C#
var device = await ParticleCloud.SharedInstance.GetDeviceAsync("53fa73265066544b16208184");
```

#### Rename a device
you can simply set the `.name` property or use -rename() method if you need a completion block to be called (for example updating a UI after renaming was done):
 
```C#
var success = await ParticleDevice.SetNameAsync("New Name");
```

#### Logout
Also clears user session and access token

```C#
ParticleCloud.SharedInstance.Logout();
```

#### ParticleEvents
Coming Soon!!

### OAuth client configuration

If you're creating an app you're required to provide the `ParticleCloud` class with OAuth clientId and secret.
Those are used to identify users coming from your specific app to the Particle Cloud.
Please follow the procedure decribed [in our guide](https://docs.particle.io/guide/how-to-build-a-product/web-app/#creating-an-oauth-client) to create those strings,
then in your platform specific launching class you can supply those credentials by setting the following properties in `ParticleCloud` singleton:


```C#
ParticleCloud.SharedInstance.OAuthClientId = "ClientID";
ParticleCloud.SharedInstance.OAuthClientSecret = "ClientID";
```

**Important**
Those credentials should be kept as secret. It is essentially a key value store for enviroment and application keys.
It's a good security practice to keep production keys out of developer hands. 

## Installation

## Communication

- If you **need help**, use [Our community website](http://community.particle.io), use the `Mobile` category for dicussion/troubleshooting iOS apps using the Particle iOS Cloud SDK.
- If you are certain you **found a bug**, _and can provide steps to reliably reproduce it_, open an issue, label it as `bug`.
- If you **have a feature request**, open an issue with an `enhancement` label on it
- If you **want to contribute**, submit a pull request, be sure to check out spark.github.io for our contribution guidelines, and please sign the [CLA](https://docs.google.com/a/particle.io/forms/d/1_2P-vRKGUFg5bmpcKLHO_qNZWGi5HKYnfrrkd-sbZoA/viewform).

## Maintainers

- Michael Watson [Github](https://www.github.com/michael-watson) | [Twitter](https://www.twitter.com/threebrewmates)

## License

Particle Xamarin Cloud SDK is available under the Apache License 2.0. See the LICENSE file for more info.