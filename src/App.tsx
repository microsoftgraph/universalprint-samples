// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. 

import React from 'react';
import './App.css';
import { login, tokenResponse, logout } from './auth/authUtil';
import { UniversalPrint } from './universalprint/universalprint';

const App: React.FunctionComponent<{}> = () => {

  const [authToken, setAuthToken] = React.useState<string>('');
  const [user, setUser] = React.useState<string>();
  const [showPrintDialog, setShowPrintDialog] = React.useState<boolean>(false);
  const [error, setError] = React.useState<String>();

  const onSignIn = async () => {
    try {
      const loginResp = await login();
      setAuthToken(loginResp.accessToken);
      setUser(loginResp.account.userName);
    } catch (error) {
      setError((error as Error).message);
    }
  };

  const onSignOut = () => {
    logout();
    setAuthToken('');
    setUser(undefined);
  }

  const onPrint = async () => {
    const response = await tokenResponse();
    setAuthToken(response.accessToken);
    setShowPrintDialog(true);
  };

  const onDialogClose = () => {
    setShowPrintDialog(false);
  }

  return (
    <section>
      <h1>
        Welcome to the Microsoft Universal Print Graph API demo
      </h1>
      {user === undefined ? (
        <button className="button" onClick={onSignIn}>Sign In</button>
      ) : (
          <>
            <button className="button" onClick={onSignOut}>
              Sign Out
              </button>
            <button className="button" onClick={onPrint}>
              Print
              </button>
          </>
        )}
      {error && <p className="error">Error: {error}</p>}
      {showPrintDialog && <UniversalPrint authToken={authToken} onClose={onDialogClose}></UniversalPrint>}
    </section>
  );

};

export default App;
