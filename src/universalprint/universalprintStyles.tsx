// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. 

import { IDropdownStyles, IStackTokens, ISpinButtonStyles, IIconProps } from '@fluentui/react';

export const stackTokens: IStackTokens = { childrenGap: 30 };
export const settingsGap: IStackTokens = { childrenGap: 10 };
export const settingStyles: Partial<IDropdownStyles> = { dropdown: { flex: 2 }, root: { display: 'flex' }, label: { flex: 1 } };
export const dropdownStyles: Partial<IDropdownStyles> = { dropdown: { width: 280 } };
export const spinStyles: Partial<ISpinButtonStyles> = { spinButtonWrapper: { flex: 2 }, root: { display: 'flex' }, labelWrapper: { flex: 1 } };
export const iconButtonStyles = {
    root: {
        marginLeft: 'auto',
        marginTop: '4px',
        marginRight: '2px',
    },
};
export const cancelIcon: IIconProps = { iconName: 'Cancel' };
