import * as Dyn from 'bluemoon-dyn-lib';
import { CheckAny } from './CheckAny';
export class CheckTextArea extends CheckAny {
    constructor(p: Dyn.IBaseComponentProps) {
        super('textarea', p);
    }
}