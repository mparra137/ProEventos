import { DatePipe } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';
import { Constants } from '@app/util/constants';

@Pipe({
  name: 'DateTimeFormatPipe'
})
export class DateTimeFormatPipe extends DatePipe implements PipeTransform {

  public override transform(value: any, args?: any): any {
    return super.transform(value, Constants.DATE_FMT);
  }

}
