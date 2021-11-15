# Universal Print Graph API Sample

This directory contains a sample web app that uses the [Microsoft Graph API](https://aka.ms/UPGraphDocs) to:
- List printer shares
- Print documents

## Prerequisites
- Before using this sample, you need to onboard to [Universal Print](https://aka.ms/UPDocs).
- Application has scopes provisioned as per the requirements of each API it calls.
- Application is already provisioned in the Azure AD tenant under which printer and print jobs need to be accessed. Be sure to add the Client ID, Tenant ID and authority URL from AAD to `.\src\authConfig.js`.

## Dependencies
- Node

## Build
1. `npm install`
2. `npm start`

## Learn more
- [Universal Print](https://aka.ms/UPDocs)
- [MSAL](https://aka.ms/MSAL)
- [MSAL with React Tutorial](https://docs.microsoft.com/en-us/azure/active-directory/develop/tutorial-v2-react)
- [Graph](https://aka.ms/UPGraphDocs)