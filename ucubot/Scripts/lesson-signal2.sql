USE ucubot;
alter table lesson_signal add column student_id INT UNSIGNED;
ALTER TABLE lesson_signal ADD constraint fk_student_id foreign key (student_id) references student(id) on delete restrict on update restrict;
