import * as Dyn from 'bluemoon-dyn-lib';
import { SectionTitle, Seperator, SubTitle, Title } from './Simples';
import { PairButton } from './PairButton';
import { CheckTextBox, CheckDblTextBox, DblTextBox } from './CheckTextBox';
import { CheckTextArea } from './CheckTextArea';
import { ComboTextBox } from './ComboTextBox';
import { CheckRadio } from './CheckRadio';
import { FileSelector } from './FileSelector';
import MultipleChoice from './MultipleChoice';

Dyn.DynConfig.exportControls({
    'seperator': Seperator,
    'title': Title,
    'subtitle': SubTitle,
    'pairbutton': PairButton,
    'sectiontitle': SectionTitle,
    'checktextbox': CheckTextBox,
    'checkdbltextbox': CheckDblTextBox,
    'combotextbox': ComboTextBox,
    'checkradio': CheckRadio,
    'checkarea': CheckTextArea,
    'dbltextbox': DblTextBox,
    'fileselector': FileSelector,
    'listbox': MultipleChoice

});