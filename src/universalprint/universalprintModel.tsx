// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. 

import { IDropdownOption } from "@fluentui/react";
import { IIntegerRange } from "../graph/graphModel";

export enum PrintingState {
    initilization,
    configuration,
    submitting,
    success,
    failure
}

export interface IPrinterSettings {
    orientationOptions?: IDropdownOption[];
    copies?: IIntegerRange;
    colorConfigOptions?: IDropdownOption[];
}

export interface ISelectedConfig {
    printer?: string;
    copies?: number;
    file?: File;
    orientation?: string;
    colorConfig?: string;
}

export interface IPrintSettingsState {
    printingState: PrintingState;
    printers: IDropdownOption[];
    settings: IPrinterSettings;
    selectedData: ISelectedConfig;
    error?: string;
}

export interface IPrintProps {
    authToken: string;
    onClose(): void;
}

export enum Action {
    ChangePrintSettings,
    ChangeState,
    ChangePrinter,
    ChangeFile,
    ChangeCopies,
    ChangeOrientation,
    ChangeColor,
    UpdateError
}

export interface IChangeEvent {
    action: Action;
    value: any;
}
