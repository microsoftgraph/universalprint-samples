# universalprint-react-sample

The directory contains a sample printing application. This sample is provided to demonstrate how developers can use the `Microsoft Graph Universal Print API` to enable cloud based printing in their Web applications.

This sample was built using [Create React App](https://github.com/facebook/create-react-app). More info for Create React App can be found [here](./CRA_README.md).

## Prerequisites
Before using this sample, you need to onboard to [`universal print](https://docs.microsoft.com/en-us/universal-print/) and ensure that:

- The Universal Print printer has been shared.
- The user has been added to the permissions of Universal Print printer.
- The user has been assigned the license to use Universal Print. For more information about assigning licenses, see Assign or remove licenses in the Azure Active Directory portal.
  
## Build Instructions
1. Run `npm install` in the root of the repo to install npm dependencies and bootstrap packages
2. Run `npm run build` to build the sample.
3. Run `npm start`. This will launch the sample application in your browser (if not, navigate to http://localhost:3000).

## Additional  Information
This sample has dependency on [`@azure/msal-browser`](https://github.com/AzureAD/microsoft-authentication-library-for-js/tree/dev/lib/msal-browser) and [`@fluentui/react`](https://github.com/microsoft/fluentui) packages. 

`@azure/msal-browser` is a Microsoft authentication library and used to enable authentication in this sample.
`@fluentui/react` is a Microsoft UX framework and used to develop this sample UI components. 