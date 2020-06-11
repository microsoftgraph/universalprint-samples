// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. 

export const hostUrl = 'https://graph.microsoft.com/beta';

export interface IGraphPrinterCapabilitiesResponse {
    value: IPrinterCapabilities;
}

export interface IPrinterCapabilities {
    copiesPerJob?: IIntegerRange;
    orientations?: IPrintOrientation[];
    colorModes?: IPrintColorConfiguration[];
}

export interface IPrinterDefaults {
    copiesPerJob?: number;
    orientation?: IPrintOrientation;
    colorMode?: IPrintColorConfiguration;
}

export interface IPrinterShare {
    id: string;
    name: string;
    defaults: IPrinterDefaults;
    capabilities: IPrinterCapabilities;
}

export interface IGraphPrinterSharesResponse {
    value: IPrinterShare[];
}

export interface IPrintJobStatus {
    processingState: JobstatusState;
    processingStateDescription: string;
    acquiredByPrinter: boolean;
}

export interface IDocumentConfiguration {
    orientation?: IPrintOrientation;
    copies?: number;
    colorMode?: IPrintColorConfiguration;
}

export interface IPrintDocument {
    id: string;
    displayName: string;
    contentType: string;
    size: number;
    configuration: IDocumentConfiguration;
}

export interface IPrintJob {
    id: string;
    createdDateTime: string;
    documents: IPrintDocument[];
    status: IPrintJobStatus;
}

export type IIntegerRange = {
    'start': number;
    'end': number;
};

export type IPrintOrientation = 'landscape' | 'portrait' | 'reverseLandscape' | 'reversePortrait';

export type IPrintColorConfiguration = 'blackAndWhite' | 'grayscale' | 'color' | 'auto';

export type JobstatusState = 'unknown'
    | 'pending'
    | 'pendingHeld'
    | 'processing'
    | 'paused'
    | 'stopped'
    | 'completed'
    | 'canceled'
    | 'aborted';