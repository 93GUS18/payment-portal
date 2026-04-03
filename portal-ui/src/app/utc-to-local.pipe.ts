import { Pipe, PipeTransform } from '@angular/core';
import * as moment from 'moment';

@Pipe({
  name: 'utcToLocal',
  standalone: true
})
export class UtcToLocalPipe implements PipeTransform {

  transform(value: string): string {
    if (!value) return '';
    return moment.utc(value).local().format('ll LT');
  }

}