// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. 

import { IDropdownOption } from "@fluentui/react";
import { IPrinterCapabilities, IPrinterDefaults, IDocumentConfiguration, IPrintOrientation, IPrintColorConfiguration, IIntegerRange, IPrinterShare } from "../graph/graphModel";
import { IPrinterSettings, ISelectedConfig, IPrintSettingsState, PrintingState, IChangeEvent, Action } from "./universalprintModel";
import { getPrinterShares, createJob, uploadData, startPrintJob } from "../graph/graphUtil";


export const initilizePrintSettings = async (authToken: string, dispatch: React.Dispatch<IChangeEvent>) => {
    try {
        const printShares = await getPrinterShares(authToken);
        const printerOptions = printShares.map((value: IPrinterShare) => {
            return {
                key: value.id,
                text: value.name,
                data: value,
            } as IDropdownOption;
        }).sort(printerComparer);
        const settings = convertToPrinterSettings((printerOptions[0].data as IPrinterShare).capabilities);
        const selectedData = convertToSelectedConfig((printerOptions[0].key as string), (printerOptions[0].data as IPrinterShare).defaults);
        const settingsSate = {
            printers: printerOptions,
            selectedData: selectedData,
            settings: settings,
            printingState: PrintingState.configuration
        } as IPrintSettingsState;
        dispatch({ action: Action.ChangePrintSettings, value: settingsSate });
    }
    catch (error) {
        dispatch({ action: Action.ChangePrintSettings, value: errorPrintSettingsState });
    }
};

export const SubmitPrintJob = async (selectedData: ISelectedConfig, authToken: string, dispatch: React.Dispatch<IChangeEvent>) => {
    if (selectedData.file) {
        const reader = new FileReader();
        reader.readAsArrayBuffer(selectedData.file);
        reader.onload = async () => {
            try {
                if (selectedData.printer) {
                    const arrayBuffer = reader.result as ArrayBuffer;
                    const array = new Uint8Array(arrayBuffer);
                    const job = await createJob(selectedData.printer, array.length, convertToDocumentConfig(selectedData), authToken);
                    await uploadData(selectedData.printer, job, authToken, array);
                    const jobStatus = await startPrintJob(selectedData.printer, job.id, authToken);
                    if (jobStatus.processingState === 'completed' || jobStatus.processingState === 'processing') {
                        dispatch({ action: Action.ChangeState, value: PrintingState.success });
                    }
                    else {
                        dispatch({ action: Action.ChangeState, value: PrintingState.failure });
                    }
                }
            }
            catch (error) {
                console.log(JSON.stringify(error));
                dispatch({ action: Action.ChangeState, value: PrintingState.failure });
            }
        }; // onload
        dispatch({ action: Action.ChangeState, value: PrintingState.submitting });
    }
};

export const onChangeState = (prevState: IPrintSettingsState, { action, value }: IChangeEvent): IPrintSettingsState => {
    if (action === Action.ChangePrintSettings) {
        return value as IPrintSettingsState;
    }
    else if (action === Action.ChangeState) {
        const state = value as PrintingState;
        return { ...prevState, printingState: state, error: state === PrintingState.submitting ? undefined : prevState.error };
    } else if (action === Action.ChangePrinter) {
        return value as IPrintSettingsState;
    } else if (action === Action.ChangeColor) {
        return { ...prevState, selectedData: { ...prevState.selectedData, colorConfig: value as string } };
    } else if (action === Action.ChangeCopies) {
        return { ...prevState, selectedData: { ...prevState.selectedData, copies: (parseInt(value as string, 10)) } };
    } else if (action === Action.ChangeOrientation) {
        return { ...prevState, selectedData: { ...prevState.selectedData, orientation: value as string } };
    }
    else if (action === Action.ChangeFile) {
        const file = value as File;
        console.log(file.name);
        if (!file.name.endsWith('oxps')) {
            return { ...prevState, selectedData: { ...prevState.selectedData, file: value as File }, error: "select a oxps type file for printing" };
        }
        else {
            return { ...prevState, selectedData: { ...prevState.selectedData, file: value as File }, error: undefined };
        }
    }
    else if (action === Action.UpdateError) {
        return { ...prevState, error: value as string };
    }
    else {
        return prevState; //Reset Information
    }
};

export const initialState: IPrintSettingsState = {
    selectedData: {},
    settings: {},
    printingState: PrintingState.initilization,
    printers: [],
};

export const errorPrintSettingsState: IPrintSettingsState = {
    selectedData: {},
    settings: {},
    printingState: PrintingState.failure,
    printers: []
};

export const StaticPrintSettings: IPrintSettingsState = {
    printingState: PrintingState.configuration,
    printers: [{ key: "printer1", text: 'printer1' },
    { key: 'printer2', text: 'printer2' }] as IDropdownOption[],
    settings: {
        copies: { minimum: 1, maximum: 100 } as IIntegerRange,
        orientationOptions: [{ key: 'portrait', text: 'portrait' }, { key: 'landscape', text: 'landscape' }] as IDropdownOption[],
        colorConfigOptions: [{ key: 'greyscale', text: 'greyscale' }, { key: 'color', text: 'color' }] as IDropdownOption[]
    } as IPrinterSettings,
    selectedData: {
        printer: 'printer1',
        copies: 1,
        orientation: 'portrait',
        colorConfig: 'greyscale'
    }
};

export const convertToPrinterSettings = (capabilities: IPrinterCapabilities | undefined): IPrinterSettings => {

    const printerOptions: IPrinterSettings = {}

    if (capabilities && capabilities.supportedCopiesPerJob) {
        printerOptions.copies = capabilities.supportedCopiesPerJob;
    }

    if (capabilities && capabilities.supportedOrientations.length) {
        printerOptions.orientationOptions = capabilities.supportedOrientations.map((orientation: string) => (
            { key: orientation, text: orientation } as IDropdownOption));
    }

    if (capabilities && capabilities.supportedColorConfigurations.length) {
        printerOptions.colorConfigOptions = capabilities.supportedColorConfigurations.map((colorConfig: string) => (
            { key: colorConfig, text: colorConfig } as IDropdownOption));
    }

    return printerOptions;
};

export const printerComparer = (option1: IDropdownOption, option2: IDropdownOption) => {
    if (option1.text > option2.text) {
        return 1;
    } else if (option1.text < option2.text) {
        return -1;
    } else { return 0; }
};

export const convertToSelectedConfig = (printerId: string, defaults: IPrinterDefaults): ISelectedConfig => {
    const ret: ISelectedConfig = {
        printer: printerId,
        copies: defaults.copiesPerJob,
        orientation: defaults.orientation as string,
        colorConfig: defaults.printColorConfiguration as string
    };

    return ret;
}

export const convertToDocumentConfig = (selData: ISelectedConfig): IDocumentConfiguration => {
    const temp: IDocumentConfiguration = { copies: 1 };
    if (selData.copies) {
        temp.copies = selData.copies;
    }
    if (selData.orientation) {
        temp.orientation = selData.orientation as IPrintOrientation;
    }

    if (selData.colorConfig) {
        temp.colorConfiguration = selData.colorConfig as IPrintColorConfiguration;
    }

    return temp;
};