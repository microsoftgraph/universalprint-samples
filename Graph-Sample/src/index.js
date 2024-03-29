import React from "react";
import ReactDOM from "react-dom";
import "bootstrap/dist/css/bootstrap.min.css";
import "./styles/index.css";
import App from "./App.jsx";
import { PublicClientApplication, InteractionType, AccountInfo } from "@azure/msal-browser";
import { MsalProvider } from "@azure/msal-react";
import { msalConfig, loginRequest } from "./authConfig";
//import { AuthCodeMSALBrowserAuthenticationProvider, AuthCodeMSALBrowserAuthenticationProviderOptions } from "@microsoft/microsoft-graph-client/authProviders/authCodeMsalBrowser";
//import { Client } from "@microsoft/microsoft-graph-client";

/**
 * Initialize a PublicClientApplication instance which is provided to the MsalProvider component
 * We recommend initializing this outside of your root component to ensure it is not re-initialized on re-renders
 */
const msalInstance = new PublicClientApplication(msalConfig);
/*
const options = {
    account: account,
    InteractionType: InteractionType.Popup,
    scopes: loginRequest
};

const authProvider = new AuthCodeMSALBrowserAuthenticationProvider(msalInstance, options);

export const graphClient = Client.initWithMiddleware({
    authProvider
});
*/

/**
 * We recommend wrapping most or all of your components in the MsalProvider component. It's best to render the MsalProvider as close to the root as possible.
 */
ReactDOM.render(
    <React.StrictMode>
        <MsalProvider instance={msalInstance}>
            <App />
        </MsalProvider>
    </React.StrictMode>,
    document.getElementById("root")
);
