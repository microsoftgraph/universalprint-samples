// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. 

import React from 'react';
import './universalprint.css';
import { Fabric, Modal, Dropdown, IDropdownOption, DefaultButton, IButtonStyles, IconButton, initializeIcons, SpinnerSize, Spinner, SpinButton, Stack, Label } from '@fluentui/react';
import { iconButtonStyles, cancelIcon, stackTokens, dropdownStyles, settingsGap, spinStyles, settingStyles } from './universalprintStyles';
import { IPrinterShare } from '../graph/graphModel';
import { initialState, StaticPrintSettings, SubmitPrintJob, initilizePrintSettings, onChangeState, convertToSelectedConfig, convertToPrinterSettings } from "./universalprintUtil"
import { PrintingState, IPrintSettingsState, Action, IPrintProps } from "./universalprintModel"

// Initialize icons
initializeIcons();

export const UniversalPrint: React.FunctionComponent<IPrintProps> = ({ authToken, onClose }: IPrintProps) => {

    const [settingsState, dispatch] = React.useReducer(onChangeState, initialState);
    const { selectedData, settings, printingState: state, printers, error } = settingsState;

    React.useEffect(() => {
        if (state === PrintingState.initilization) {
            // initilizePrintSettings(authToken, dispatch);
            dispatch({ action: Action.ChangePrintSettings, value: StaticPrintSettings });
        }
    }, [state, authToken]);

    const onPrinterOptionsChange = async (event: React.FormEvent<HTMLDivElement>, option?: IDropdownOption | undefined, index?: number | undefined) => {
        if (option !== undefined && selectedData && settings) {
            const state: IPrintSettingsState = {
                ...settingsState,
                selectedData: convertToSelectedConfig(option.key as string, (option.data as IPrinterShare).defaults),
                settings: convertToPrinterSettings((option.data as IPrinterShare).capabilities)
            };

            dispatch({ action: Action.ChangePrinter, value: state });
        }
    }

    const onSubmit = async () => {
        if (selectedData.file && selectedData.printer && error === undefined) {
            SubmitPrintJob(selectedData, authToken, dispatch);
        } else {
            dispatch({ action: Action.UpdateError, value: "Select a file to be printed" });
        }
    }

    const onOrientationChange = (event: React.FormEvent<HTMLDivElement>, option?: IDropdownOption | undefined, index?: number | undefined) => {
        if (option !== undefined && selectedData) {
            dispatch({ action: Action.ChangeOrientation, value: option.key as string });
        }
    }

    const onColorConfigChange = (event: React.FormEvent<HTMLDivElement>, option?: IDropdownOption | undefined, index?: number | undefined) => {
        if (option !== undefined && selectedData) {
            dispatch({ action: Action.ChangeColor, value: option.key as string });
        }
    }

    const onFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        if (event.target.files && selectedData) {
            dispatch({ action: Action.ChangeFile, value: event.target.files[0] });
        }
    }

    const onCopiesChange = (value: string, event?: React.SyntheticEvent<HTMLElement, Event> | undefined) => {
        let count = Number(value);
        count = (isNaN(count) || count < 0) ? 1 : count;
        if (selectedData) {
            dispatch({ action: Action.ChangeCopies, value: count });
        }
    }

    return (
        <Fabric>
            <Modal
                isOpen={true}
                isBlocking={true}
                containerClassName="universalPrintcontainer"
                scrollableContentClassName='scrollableClass'
            >
                <Stack className='stateStack' verticalAlign='center'>
                    <IconButton styles={iconButtonStyles} iconProps={cancelIcon} ariaLabel="Close popup modal" onClick={onClose} className='universalPrintCancelIcon' />

                    {state === PrintingState.initilization && (
                        <Stack className='state' verticalAlign='center' horizontalAlign='center'>
                            <Spinner label='Loading print settings' size={SpinnerSize.large} />
                        </Stack>
                    )}

                    {(state === PrintingState.configuration) && selectedData && settings && (
                        <Stack className='stateStack' horizontalAlign="space-around" verticalAlign='start' tokens={stackTokens}>
                            {error && <Label className='error'> {error} </Label>}

                            <Dropdown selectedKey={selectedData.printer} label='Printer' /* onChange={onPrinterOptionsChange} */ options={printers} styles={dropdownStyles} />

                            <Stack>
                                <Label required={true}>File: </Label>
                                <input type='file' onChange={onFileChange} key={selectedData.printer} />
                            </Stack>

                            <Stack tokens={settingsGap}>
                                <Label> Job Settings </Label>
                                {selectedData.copies && <SpinButton styles={spinStyles} value={selectedData.copies.toString(10)} label='Copies' min={settings.copies?.minimum} max={settings.copies?.maximum} step={1} onValidate={onCopiesChange} />}
                                {settings.orientationOptions && <Dropdown selectedKey={selectedData.orientation} label='Orientation' options={settings.orientationOptions} styles={settingStyles} onChange={onOrientationChange} />}
                                {settings.colorConfigOptions && <Dropdown defaultSelectedKey={selectedData.colorConfig} label='Color' options={settings.colorConfigOptions} styles={settingStyles} onChange={onColorConfigChange} />}
                            </Stack>

                            <Stack horizontal horizontalAlign='end'>
                                <DefaultButton className="button" onClick={onClose}> Cancel </DefaultButton>
                                <DefaultButton styles={{ root: { marginRight: 0 } } as IButtonStyles} /* onClick={onSubmit} */> Print </DefaultButton>
                            </Stack>
                        </Stack>
                    )}

                    {state === PrintingState.submitting && (
                        <Stack className='state' verticalAlign='center' horizontalAlign='center'>
                            <Spinner label='Submitting Print Job' size={SpinnerSize.large} />
                        </Stack>
                    )}

                    {state === PrintingState.success && (
                        <Stack className='state' verticalAlign='center' horizontalAlign='center'>
                            <Label> Successfully submitted Print Job </Label>
                        </Stack>
                    )}

                    {state === PrintingState.failure && (
                        <Stack className='state' verticalAlign='center' horizontalAlign='center'>
                            <Label> Failed to do printing </Label>
                        </Stack>
                    )}
                </Stack>
            </Modal>
        </Fabric >
    );
};

