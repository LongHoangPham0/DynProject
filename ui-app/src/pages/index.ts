import * as Dyn from 'bluemoon-dyn-lib';
import { ListItem } from "./ItemList";
import { SubItem } from "./SubItemList.SubItem";
import { SelectionItem } from "./SelectionList";
import { ItemApproval, TabExt } from "./ItemDetail";
import { ValidationDef } from './ValidationDef';
import { ConstraintDef, FieldConditions, FieldConditionAddButton, FieldConditionRemoveButton } from './ConstraintDef';
import { Query, QueryAddButton, QueryRemoveButton } from './Queries';
import { ReportItem } from './ReportList';
import { HistoryDetail } from './ItemHistory';
import { RuleItem } from './RuleList';
import { ItemLauncher, ReportLauncher, RuleLauncher } from './Launchers';
import { RuleAction, ActionAddButton, ActionRemoveButton } from './RuleActions';
import { AttachmentDetail } from './AttachmentList';
import { SortingList, SortingAddButton, SortingRemoveButton } from './Sorting';
import { MapItem, MapToItem } from './MapToItem';



Dyn.DynConfig.exportPages({
    'ValidationDef': ValidationDef,
    'ConstraintDef': ConstraintDef,
    'ItemLauncher': ItemLauncher,
    'ReportLauncher': ReportLauncher,
    'RuleLauncher': RuleLauncher,
    'MapToItem': MapToItem

    
});

Dyn.DynConfig.exportControls({
    'ListItem': ListItem,
    'FieldConditions': FieldConditions,
    'FieldConditionAddButton': FieldConditionAddButton,
    'FieldConditionRemoveButton': FieldConditionRemoveButton,
    'SubItem': SubItem,
    'SelectionItem': SelectionItem,
    'Tab': TabExt,
    'HistoryDetail': HistoryDetail,
    'Query': Query,
    'QueryRemoveButton': QueryRemoveButton,
    'QueryAddButton': QueryAddButton,
    'ReportItem': ReportItem,
    'ItemApproval': ItemApproval,
    'RuleItem': RuleItem,
    'RuleAction': RuleAction,
    'ActionRemoveButton': ActionRemoveButton,
    'ActionAddButton': ActionAddButton,
    'AttachmentDetail': AttachmentDetail,
    'SortingList': SortingList,
    'SortingAddButton': SortingAddButton,
    'SortingRemoveButton': SortingRemoveButton,
    'MapItem': MapItem
});