import { DatePipe } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';
import { Constants } from '@app/util/constants';
import { timepickerReducer } from 'ngx-bootstrap/timepicker/reducer/timepicker.reducer';

@Pipe({
  name: 'DateTimeFormatPipe'
})
export class DateTimeFormatPipe extends DatePipe implements PipeTransform {

  public override transform(value: any, args?: any): any {  
    let _dateTime = value.split(' ');
    let datePart = _dateTime[0].split('/');
    let timePart = _dateTime[1].split(':');    
    let newDate = new Date(datePart[2], datePart[1]-1, datePart[0], timePart[0], timePart[1], timePart[2])
    return super.transform(newDate, Constants.DATE_TIME_FMT);
  }

}
