// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. 

import * as msal from "@azure/msal-browser";
import { TokenResponse } from "@azure/msal-common";


const loginRequestScopes = {
    scopes: ["user.read"] // optional Array<string>
};

const msalConfig = {
    auth: {
        clientId: '789f472e-84b4-499e-9381-c5dcb7a21a52',
        authority: 'https://login.microsoftonline.com/common'
    }
};

export const msalInstance = new msal.PublicClientApplication(msalConfig);

export const login = async (): Promise<TokenResponse> => {
    return msalInstance.loginPopup(loginRequestScopes);
}

export const logout = async () => {
    msalInstance.logout();
}

export const tokenResponse = async () => await msalInstance.acquireTokenSilent(loginRequestScopes).catch(async (error) => {
    if (requiresInteraction(error.errorCode)) {
        // fallback to interaction when silent call fails
        return await msalInstance.acquireTokenPopup(loginRequestScopes);
    } else {
        throw error;
    }
});

export const requiresInteraction = (errorMessage: string) => {
    if (!errorMessage || !errorMessage.length) {
        return false;
    }

    return (
        errorMessage.indexOf("consent_required") > -1 ||
        errorMessage.indexOf("interaction_required") > -1 ||
        errorMessage.indexOf("login_required") > -1
    );
};


